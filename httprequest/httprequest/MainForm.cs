using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Web;
using System.Windows.Forms;

#pragma warning disable CS8509

namespace httprequest
{
    public partial class MainForm : Form
    {
        #region API key
        private const string apiKey = "";
        #endregion

        private string uri;

        public MainForm() => InitializeComponent();

        private void MainForm_Load(object sender, EventArgs e)
        {
            uri = textBoxURI.Text;
            parametersView.Rows.Add(true, "Header", "Accept", "application/json");
            parametersView.Rows[0].ReadOnly = true;
            parametersView.Rows.Add(true, "Query", "apiKey", apiKey);
            parametersView.Rows[1].ReadOnly = true;
            parametersView.Rows.Add(true, "Variable", "stopNo", 53745);
        }

        private void TextBoxURI_TextChanged(object sender, EventArgs e) => uri = textBoxURI.Text;

        private async void ButtonSend_Click(object sender, EventArgs e)
        {
            textBoxURI.Enabled = false;
            parametersView.Enabled = false;
            buttonSend.Enabled = false;
            using HttpClient client = new();
            NameValueCollection query = HttpUtility.ParseQueryString("");
            foreach (DataGridViewRow row in parametersView.Rows)
            {
                if (row.Cells[0].Value == null || !(bool)row.Cells[0].Value)
                    continue;
                string type = Stringify(row.Cells[1].Value);
                string key = Stringify(row.Cells[2].Value);
                string value = Stringify(row.Cells[3].Value);
                if (String.IsNullOrEmpty(key))
                    continue;
                if (type.Equals("Header"))
                    client.DefaultRequestHeaders.Add(key, value);
                else if (type.Equals("Query"))
                    query.Add(key, value);
                else if (type.Equals("Variable"))
                {
                    int index = uri.IndexOf($"${key}$");
                    if (index > -1)
                    {
                        uri = uri.Remove(index, key.Length + 2);
                        uri = uri.Insert(index, value);
                    }
                }
            }
            string json = await client.GetStringAsync($"{uri}?{query}");
            json = json.Substring(2, json.Length - 4);
            MessageBox.Show(json);
            responseView.BeginUpdate();
            int tmp = 0;
            while ((tmp = json.IndexOf('\\')) != -1)
                json = json.Remove(tmp, 1);
            int first = 0, last = -1, end = 0, depth = 0;
            bool[] array = { false, false, false };
            TreeNodeCollection nodes = responseView.Nodes;
            while (json.IndexOf('"', last + 1) != -1)
            {
                end = json.IndexOfAny("}]".ToCharArray(), first);
                first = json.IndexOf('"', last + 1);
                last = json.IndexOf('"', first + 1);
                string key = "";
                string value = "";
                if (end < first && end != -1)
                {
                    last = end;
                    nodes = --depth switch
                    {
                        0 => responseView.Nodes,
                        1 => responseView.Nodes[responseView.Nodes.Count - 1].Nodes,
                    };
                    MessageBox.Show($"{depth}");
                    if (!(array[depth] && json[last + 2] == '{'))
                        continue;
                }
                else
                    key = json.Substring(first + 1, last - first - 1);
                first = last + 2;
                switch (json[first])
                {
                case '[':
                    if (array[depth])
                        nodes.Add($"[{nodes.Count}]");
                    else
                        nodes.Add($"{key}");
                    array[++depth] = true;
                    MessageBox.Show($"{depth}");
                    nodes = depth switch
                    {
                        1 => responseView.Nodes[responseView.Nodes.Count - 1].Nodes,
                        2 => responseView.Nodes[responseView.Nodes.Count - 1]
                            .Nodes[responseView.Nodes[responseView.Nodes.Count - 1].Nodes.Count - 1].Nodes
                    };
                    if (json[first + 1] == '[')
                        goto case '[';
                    else
                        goto case '{';
                case '{':
                    if (array[depth])
                        nodes.Add($"[{nodes.Count}]");
                    else
                        nodes.Add($"{key}");
                    array[++depth] = false;
                    MessageBox.Show($"{depth}");
                    nodes = depth switch
                    {
                        1 => responseView.Nodes[responseView.Nodes.Count - 1].Nodes,
                        2 => responseView.Nodes[responseView.Nodes.Count - 1]
                            .Nodes[responseView.Nodes[responseView.Nodes.Count - 1].Nodes.Count - 1].Nodes
                    };
                    break;
                case '"':
                    last = json.IndexOf('"', first + 1);
                    value = json.Substring(first + 1, last - first - 1);
                    nodes.Add($"{key}: {value}");
                    break;
                default:
                    last = json.IndexOf(',', first);
                    value = json.Substring(first, last - first);
                    nodes.Add($"{key}: {value}");
                    break;
                }
            }
            responseView.EndUpdate();
            buttonSend.Enabled = true;
            parametersView.Enabled = true;
            textBoxURI.Enabled = true;
        }

        private string Stringify(object obj)
        {
            if (!String.IsNullOrEmpty(obj as String))
                return obj as String;
            else if (obj is Int32)
                return ((int)obj).ToString();
            else
                return String.Empty;
        }
    }
}
