namespace httprequest
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSend = new System.Windows.Forms.Button();
            this.responseView = new System.Windows.Forms.TreeView();
            this.parametersView = new System.Windows.Forms.DataGridView();
            this.paramEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.paramType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.paramName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxURI = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.parametersView)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(544, -1);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 28);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.ButtonSend_Click);
            // 
            // responseView
            // 
            this.responseView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.responseView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.responseView.Location = new System.Drawing.Point(0, 225);
            this.responseView.Name = "responseView";
            this.responseView.Size = new System.Drawing.Size(618, 199);
            this.responseView.TabIndex = 3;
            this.responseView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ResponseView_NodeMouseDoubleClick);
            // 
            // parametersView
            // 
            this.parametersView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.parametersView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.parametersView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parametersView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramEnabled,
            this.paramType,
            this.paramName,
            this.paramValue});
            this.parametersView.Location = new System.Drawing.Point(0, 26);
            this.parametersView.Name = "parametersView";
            this.parametersView.RowHeadersVisible = false;
            this.parametersView.RowHeadersWidth = 62;
            this.parametersView.RowTemplate.Height = 28;
            this.parametersView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.parametersView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.parametersView.Size = new System.Drawing.Size(618, 199);
            this.parametersView.TabIndex = 2;
            // 
            // paramEnabled
            // 
            this.paramEnabled.HeaderText = "Enabled";
            this.paramEnabled.MinimumWidth = 75;
            this.paramEnabled.Name = "paramEnabled";
            this.paramEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.paramEnabled.Width = 75;
            // 
            // paramType
            // 
            this.paramType.HeaderText = "Type";
            this.paramType.Items.AddRange(new object[] {
            "Header",
            "Query",
            "Variable"});
            this.paramType.MaxDropDownItems = 3;
            this.paramType.MinimumWidth = 100;
            this.paramType.Name = "paramType";
            this.paramType.Sorted = true;
            this.paramType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.paramType.Width = 150;
            // 
            // paramName
            // 
            this.paramName.HeaderText = "Name";
            this.paramName.MinimumWidth = 8;
            this.paramName.Name = "paramName";
            this.paramName.Width = 150;
            // 
            // paramValue
            // 
            this.paramValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.paramValue.HeaderText = "Value";
            this.paramValue.MinimumWidth = 8;
            this.paramValue.Name = "paramValue";
            this.paramValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // textBoxURI
            // 
            this.textBoxURI.Location = new System.Drawing.Point(0, 0);
            this.textBoxURI.Name = "textBoxURI";
            this.textBoxURI.Size = new System.Drawing.Size(545, 26);
            this.textBoxURI.TabIndex = 0;
            this.textBoxURI.Text = "http://api.translink.ca/rttiapi/v1/stops/$stopNo$/estimates";
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(618, 424);
            this.Controls.Add(this.textBoxURI);
            this.Controls.Add(this.parametersView);
            this.Controls.Add(this.responseView);
            this.Controls.Add(this.buttonSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "HTTP GET Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.parametersView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TreeView responseView;
        private System.Windows.Forms.DataGridView parametersView;
        private System.Windows.Forms.TextBox textBoxURI;
        private System.Windows.Forms.DataGridViewCheckBoxColumn paramEnabled;
        private System.Windows.Forms.DataGridViewComboBoxColumn paramType;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramName;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramValue;
    }
}