#pragma warning disable CA1861
#pragma warning disable CA1862
#pragma warning disable CS8509
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable IDE0044
#pragma warning disable SYSLIB0014

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace p2p
{
    internal class Program
    {
        #region API key
        private const string apiKey = "";
        #endregion

        private static List<String[]> stops = [];
        private static List<String[]> trips = [];
        private static List<String[]> calendar = [];
        private static List<String[]> dates = [];

        private const int MAGIC = 2024;

        private static byte i_s_stop, i_s_name, i_s_id,
                            i_st_id, i_st_time, i_st_trip,
                            i_t_trip, i_t_name, i_t_svc,
                            i_c_svc, i_c_week,
                            i_cd_svc, i_cd_date, i_cd_excep;

        private static Random random = new();

        private static async Task Main()
        {
            if (File.Exists("last_fetch.txt"))
            {
                long ticks = 0;
                try
                {
                    ticks = long.Parse(File.ReadAllLines("last_fetch.txt")[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                DateTime lastFetch = new(ticks);
                DateTime date = DateTime.Now;
                Console.WriteLine($"Schedules last updated on {lastFetch.Year}/{lastFetch.Month:D2}/{lastFetch.Day:D2}");
                while (true)
                {
                    if ((int)date.Subtract(lastFetch).TotalDays <= 0)
                    {
                        Console.WriteLine("Schedules are up to date.");
                        break;
                    }
                    try
                    {
                        using WebClient client = new();
                        Console.WriteLine($"Attempting to download from https://gtfs-static.translink.ca/gtfs/History/{date.Year}-{date.Month:D2}-{date.Day:D2}/google_transit.zip...");
                        client.DownloadFile(new Uri($"https://gtfs-static.translink.ca/gtfs/History/{date.Year}-{date.Month:D2}-{date.Day:D2}/google_transit.zip"), "google_transit.zip");
                        DeleteFile("stops.txt");
                        DeleteFile("stop_times.txt");
                        DeleteFile("trips.txt");
                        DeleteFile("calendar.txt");
                        DeleteFile("calendar_dates.txt");
                        DeleteFile("agency.txt");
                        DeleteFile("direction_names_exceptions.txt");
                        DeleteFile("directions.txt");
                        DeleteFile("feed_info.txt");
                        DeleteFile("pattern_id.txt");
                        DeleteFile("routes.txt");
                        DeleteFile("shapes.txt");
                        DeleteFile("signup_periods.txt");
                        DeleteFile("stop_order_exceptions.txt");
                        DeleteFile("transfers.txt");
                        ZipFile.ExtractToDirectory("google_transit.zip", ".");
                        DeleteFile("agency.txt");
                        DeleteFile("direction_names_exceptions.txt");
                        DeleteFile("directions.txt");
                        DeleteFile("feed_info.txt");
                        DeleteFile("pattern_id.txt");
                        DeleteFile("routes.txt");
                        DeleteFile("shapes.txt");
                        DeleteFile("signup_periods.txt");
                        DeleteFile("stop_order_exceptions.txt");
                        DeleteFile("transfers.txt");
                        DeleteFile("google_transit.zip");
                        Console.WriteLine("Schedules successfully updated.");
                        File.WriteAllText("last_fetch.txt", DateTime.Now.Ticks.ToString());
                        break;
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("404"))
                            date = date.AddDays(-1);
                        else
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    using WebClient client = new();
                    Console.WriteLine("Downloading from https://gtfs-static.translink.ca/gtfs/google_transit.zip...");
                    client.DownloadFile("https://gtfs-static.translink.ca/gtfs/google_transit.zip", "google_transit.zip");
                    DeleteFile("stops.txt");
                    DeleteFile("stop_times.txt");
                    DeleteFile("trips.txt");
                    DeleteFile("calendar.txt");
                    DeleteFile("calendar_dates.txt");
                    DeleteFile("agency.txt");
                    DeleteFile("direction_names_exceptions.txt");
                    DeleteFile("directions.txt");
                    DeleteFile("feed_info.txt");
                    DeleteFile("pattern_id.txt");
                    DeleteFile("routes.txt");
                    DeleteFile("shapes.txt");
                    DeleteFile("signup_periods.txt");
                    DeleteFile("stop_order_exceptions.txt");
                    DeleteFile("transfers.txt");
                    ZipFile.ExtractToDirectory("google_transit.zip", ".");
                    DeleteFile("agency.txt");
                    DeleteFile("direction_names_exceptions.txt");
                    DeleteFile("directions.txt");
                    DeleteFile("feed_info.txt");
                    DeleteFile("pattern_id.txt");
                    DeleteFile("routes.txt");
                    DeleteFile("shapes.txt");
                    DeleteFile("signup_periods.txt");
                    DeleteFile("stop_order_exceptions.txt");
                    DeleteFile("transfers.txt");
                    DeleteFile("google_transit.zip");
                    Console.WriteLine("Schedules successfully downloaded.");
                    File.WriteAllText("last_fetch.txt", DateTime.Now.Ticks.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            try
            {
                using (StreamReader reader = File.OpenText("stops.txt"))
                {
                    string? read = reader.ReadLine();
                    i_s_stop = (byte)Array.IndexOf(read.Split(','), "stop_code");
                    i_s_name = (byte)Array.IndexOf(read.Split(','), "stop_name");
                    i_s_id = (byte)Array.IndexOf(read.Split(','), "stop_id");
                    while ((read = reader.ReadLine()) != null)
                        stops.Add(read.Split(','));
                }
                using (StreamReader reader = File.OpenText("stop_times.txt"))
                {
                    string? read = reader.ReadLine();
                    i_st_id = (byte)Array.IndexOf(read.Split(','), "stop_id");
                    i_st_time = (byte)Array.IndexOf(read.Split(','), "arrival_time");
                    i_st_trip = (byte)Array.IndexOf(read.Split(','), "trip_id");
                }
                using (StreamReader reader = File.OpenText("trips.txt"))
                {
                    string? read = reader.ReadLine();
                    i_t_trip = (byte)Array.IndexOf(read.Split(','), "trip_id");
                    i_t_name = (byte)Array.IndexOf(read.Split(','), "trip_headsign");
                    i_t_svc = (byte)Array.IndexOf(read.Split(','), "service_id");
                    while ((read = reader.ReadLine()) != null)
                        trips.Add(read.Split(','));
                }
                using (StreamReader reader = File.OpenText("calendar.txt"))
                {
                    string? read = reader.ReadLine();
                    i_c_svc = (byte)Array.IndexOf(read.Split(','), "service_id");
                    i_c_week = (byte)Array.IndexOf(read.Split(','), "monday");
                    while ((read = reader.ReadLine()) != null)
                        calendar.Add(read.Split(','));
                }
                using (StreamReader reader = File.OpenText("calendar_dates.txt"))
                {
                    string? read = reader.ReadLine();
                    i_cd_svc = (byte)Array.IndexOf(read.Split(','), "service_id");
                    i_cd_date = (byte)Array.IndexOf(read.Split(','), "date");
                    i_cd_excep = (byte)Array.IndexOf(read.Split(','), "exception_type");
                    while ((read = reader.ReadLine()) != null)
                        dates.Add(read.Split(','));
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            using UdpClient receiver = new();
            receiver.Client.Bind(new IPEndPoint(IPAddress.Any, MAGIC));
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Enter a route number or press ENTER for any route: ");
                    string route = Console.ReadLine().ToUpper();
                    Console.Write("Enter a stop ID or search by stop name: ");
                    string name = Console.ReadLine().ToUpper();
                    if (name.Equals(""))
                        continue;
                    ushort match = 65535;
                    if (name.Length == 5 && UInt16.TryParse(name, out _))
                    {
                        for (ushort i = 0; i < stops.Count; i++)
                            if (name.Equals(stops[i][i_s_stop]))
                                match = i;
                        if (match == 65535)
                        {
                            Console.WriteLine("Stop ID not found.");
                            continue;
                        }
                    }
                    else
                    {
                        if (name.StartsWith("WB"))
                            name = String.Concat("WESTBOUND", name.AsSpan(2));
                        if (name.StartsWith("EB"))
                            name = String.Concat("EASTBOUND", name.AsSpan(2));
                        if (name.StartsWith("NB"))
                            name = String.Concat("NORTHBOUND", name.AsSpan(2));
                        if (name.StartsWith("SB"))
                            name = String.Concat("SOUTHBOUND", name.AsSpan(2));
                        string[] stop = name.Replace(".", "").Split('@');
                        List<UInt16> matches = [];
                        if (stop.Length == 1)
                        {
                            for (ushort i = 0; i < stops.Count; i++)
                                if (stops[i][i_s_name].Replace(".", "").ToUpper().StartsWith(name)
                                    && stops[i][i_s_name].Contains(" @ ")
                                    && !stops[i][i_s_name].Contains(" @ Platform"))
                                    matches.Add(i);
                            for (ushort i = 0; i < stops.Count; i++)
                                if (stops[i][i_s_name].Replace(".", "").ToUpper().Contains(name)
                                    && stops[i][i_s_name].Contains(" @ ")
                                    && !stops[i][i_s_name].Contains(" @ Platform")
                                    && !matches.Contains(i))
                                    matches.Add(i);
                        }
                        else
                        {
                            stop[0] = stop[0].Trim();
                            stop[1] = stop[1].Trim();
                            for (ushort i = 0; i < stops.Count; i++)
                                if (stops[i][i_s_name].Contains(" @ ")
                                    && !stops[i][i_s_name].Contains(" @ Platform")
                                    && (stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[0].StartsWith(stop[0])
                                        && stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[1].StartsWith(stop[1])))
                                    matches.Add(i);
                            for (ushort i = 0; i < stops.Count; i++)
                                if (stops[i][i_s_name].Contains(" @ ")
                                    && !stops[i][i_s_name].Contains(" @ Platform")
                                    && !matches.Contains(i)
                                    && ((stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[0].StartsWith(stop[0])
                                            && stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[1].Contains(stop[1]))
                                        || stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[1].StartsWith(stop[1])
                                            && stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[0].Contains(stop[0])))
                                    matches.Add(i);
                            for (ushort i = 0; i < stops.Count; i++)
                                if (stops[i][i_s_name].Contains(" @ ")
                                    && !stops[i][i_s_name].Contains(" @ Platform")
                                    && !matches.Contains(i)
                                    && (stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[0].Contains(stop[0])
                                        && stops[i][i_s_name].Replace(".", "").ToUpper().Split('@')[1].Contains(stop[1])))
                                    matches.Add(i);
                        }
                        if (matches.Count == 0)
                        {
                            Console.WriteLine($"No matches for {name} found.");
                            continue;
                        }
                        for (ushort i = 0; i < matches.Count; i++)
                            Console.WriteLine($"{i}. {stops[matches[i]][i_s_name]}");
                        Console.Write($"{matches.Count} matches found. Select a stop or press ENTER to return to search: ");
                        string input = Console.ReadLine();
                        if (input.Equals(""))
                            continue;
                        match = matches[UInt16.Parse(input)];
                    }
                    string gtfs_id = stops[match][i_s_id];
                    Console.WriteLine($"Selected stop: {stops[match][i_s_name]}");
                    List<DateTime> times = [];
                    using (StreamReader reader = File.OpenText("stop_times.txt"))
                    {
                        reader.ReadLine();
                        DateTime time;
                        string? read = null;
                        while ((read = reader.ReadLine()) != null)
                            if (gtfs_id.Equals(read.Split(',')[i_st_id]) && IsCurrentCalendar(read.Split(',')[i_st_trip], route)
                                && (time = Time(read.Split(',')[i_st_time])).Subtract(DateTime.Now).Ticks > 0)
                                times.Add(time);
                    }
                    times.Sort(DateTime.Compare);
                    Console.WriteLine($"Next {Math.Min(5, times.Count)} scheduled bus times:");
                    for (int i = 0; i < 5 && i < times.Count; i++)
                        Console.WriteLine(times[i]);
                    if (times.Count == 0)
                        continue;
                    TimeSpan delay = times[0].Subtract(DateTime.Now.AddMinutes(4));
                    delay = delay.Add(new TimeSpan(0, 0, 0 - random.Next(121)));
                    for (int i = 0; delay.TotalSeconds < 0 || times[0].Subtract(DateTime.Now).TotalSeconds - delay.TotalSeconds < 0 && i <= 10; i++)
                        delay = TimeSpan.Zero.Add(new(0, 0, random.Next(10 - i)));
                    Console.WriteLine($"Waiting {(int)delay.TotalSeconds} seconds until {DateTime.Now.Add(delay)}");
                    IPAddress local;
                    using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
                    {
                        socket.Connect("8.8.8.8", MAGIC);
                        local = (socket.LocalEndPoint as IPEndPoint).Address;
                    }
                    CancellationTokenSource cancel = new();
                    try
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                Bitfield received;
                                ulong route_id;
                                IPEndPoint ip = new(0, 0);
                                while (true)
                                {
                                    try
                                    {
                                        byte[] buf = receiver.Receive(ref ip);
                                        if (buf == null || ip.Address.ToString().Equals("0.0.0.0"))
                                            continue;
                                        if (ip.Address.Equals(local))
                                            continue;
                                        if (buf.Length != 16)
                                            continue;
                                        byte[] data = new Bitfield(buf).FromHammingCode(out _)[1..];
                                        received = new(data);
                                        int temp = 8;
                                        if (received.Read(11, ref temp) != MAGIC)
                                            continue;
                                        temp = received.Count - 24;
                                        if (String.IsNullOrEmpty(route))
                                        {
                                            route_id = received.Read(10, ref temp);
                                            route = route_id switch
                                            {
                                                > 999 => $"R{1024 - route_id}",
                                                > 900 => $"N{1000 - route_id}",
                                                _ => route_id.ToString()
                                            };
                                        }
                                        else
                                        {
                                            route_id = route[0] switch
                                            {
                                                'R' => 1024 - UInt64.Parse(route[1..]),
                                                'N' => 1000 - UInt64.Parse(route[1..]),
                                                _ => UInt16.Parse(route)
                                            };
                                            if (!(route_id == received.Read(10, ref temp)))
                                                continue;
                                        }
                                        temp = received.Count - 14;
                                        if (!stops[match][i_s_stop].Equals((received.Read(14, ref temp) + 49152).ToString()))
                                            continue;
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        if (e is TaskCanceledException)
                                            return;
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                Console.WriteLine($"Recieved data from {ip.Address}:");
                                int index = 19;
                                // See below for structure
                                DateTime update = new(DateOnly.FromDateTime(DateTime.Now),
                                    new TimeOnly((int)received.Read(5, ref index), (int)received.Read(6, ref index)));
                                List<Byte> countdowns = [];
                                for (int i = 0; i < 6; i++)
                                    countdowns.Add((byte)received.Read(7, ref index));
                                List<Byte> schedule_stat = [];
                                for (int i = 0; i < 6; i++)
                                    schedule_stat.Add((byte)received.Read(1, ref index));
                                List<Byte> cancellations = [];
                                for (int i = 0; i < 6; i++)
                                    cancellations.Add((byte)received.Read(2, ref index));
                                List<Byte> additions = [];
                                for (int i = 0; i < 6; i++)
                                    additions.Add((byte)received.Read(1, ref index));
                                Console.WriteLine("Broadcasting the following data:");
                                Console.WriteLine($"Last updated at {update.Hour:D2}:{update.Minute:D2}");
                                for (int i = 0; i < 6 && countdowns[i] != 127; i++)
                                    Console.WriteLine($"Bus {i} in {countdowns[i]:D3} minutes, " +
                                        $"status: {(schedule_stat[i] == 1 ? "real-time" : "scheduled")}, " +
                                        $"cancelled: {cancellations[i]:B2}, added: {additions[i]}");
                                Console.WriteLine($"Route number: {route} (received: {route_id})");
                                Console.WriteLine($"Stop ID: {stops[match][i_s_stop]} (received: {UInt16.Parse(stops[match][i_s_stop]) - 49152})");
                            }
                            catch (TaskCanceledException)
                            {
                                return;
                            }
                        }).WaitAsync(delay, cancel.Token);
                    }
                    catch (TimeoutException)
                    {
                        cancel.Cancel();
                        using HttpClient client = new();
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        string json = await client.GetStringAsync($"http://api.translink.ca/rttiapi/v1/stops/{stops[match][i_s_stop]}/estimates?apiKey={apiKey}");
                        int start = 0, end = json.IndexOf($"\"RouteNo\"", start + 1);
                        do
                        {
                            Bitfield message = new(new byte[15]);
                            start = String.IsNullOrEmpty(route) ?
                                end :
                                json.IndexOf($"\"RouteNo\":\"{(UInt16.TryParse(route, out _) ? UInt16.Parse(route) : route):D3}\"");
                            if (start == -1 || start == json.Length)
                                break;
                            string route_name = route;
                            if (String.IsNullOrEmpty(route_name))
                                route_name = json.Substring(start + 11, 3).Replace("\"", "");
                            ulong route_id = route_name[0] switch
                            {
                                'R' => 1024 - UInt64.Parse(route_name[1..]),
                                'N' => 1000 - UInt64.Parse(route_name[1..]),
                                _ => UInt16.Parse(route_name)
                            };
                            end = json.IndexOf($"\"RouteNo\"", start + 1);
                            end = end == -1 ? json.Length : end;
                            DateTime update = DateTime.Parse(json.Substring(json.IndexOf(':', json.IndexOf("LastUpdate")) + 2, 11));
                            List<Byte> countdowns = [];
                            for (int i = json.IndexOf("ExpectedCountdown", start); i < end && i != -1; i = json.IndexOf("ExpectedCountdown", i + 1))
                            {
                                byte countdown = 0;
                                for (int j = json.IndexOf(':', i) + 1; json[j] >= '0' && json[j] <= '9'; j++)
                                    countdown = (byte)(countdown * 10 + (json[j] - '0'));
                                countdowns.Add(countdown);
                            }
                            if (countdowns.Count == 0)
                                break;
                            while (countdowns.Count < 6)
                                countdowns.Add(127);
                            List<Byte> schedule_stat = [];
                            for (int i = json.IndexOf("ScheduleStatus", start); i < end && i != -1; i = json.IndexOf("ScheduleStatus", i + 1))
                                schedule_stat.Add((byte)(json[json.IndexOf(':', i) + 2] == '*' ? 0 : 1));
                            while (schedule_stat.Count < 6)
                                schedule_stat.Add(0);
                            List<Byte> cancellations = [];
                            for (int i = json.IndexOf("CancelledTrip", start); i < end && i != -1; i = json.IndexOf("CancelledTrip", i + 1))
                                cancellations.Add((byte)(json[json.IndexOf(':', i) + 1] == 'f' ? 0 : 2));
                            while (cancellations.Count < 6)
                                cancellations.Add(0);
                            for (int i = json.IndexOf("CancelledStop", start), j = 0; i < end && i != -1 && j < 6; i = json.IndexOf("CancelledStop", i + 1), j++)
                                cancellations[j] |= (byte)(json[json.IndexOf(':', i) + 1] == 'f' ? 0 : 1);
                            List<Byte> additions = [];
                            for (int i = json.IndexOf("AddedTrip", start); i < end && i != -1; i = json.IndexOf("AddedTrip", i + 1))
                                additions.Add((byte)(json[json.IndexOf(':', i) + 1] == 'f' ? 0 : 2));
                            while (additions.Count < 6)
                                additions.Add(0);
                            for (int i = json.IndexOf("AddedStop", start), j = 0; i < end && i != -1 && j < 6; i = json.IndexOf("AddedStop", i + 1), j++)
                                additions[j] |= (byte)(json[json.IndexOf(':', i) + 1] == 'f' ? 0 : 1);
                            byte direction = json[json.IndexOf(':', json.IndexOf("Pattern", start)) + 2] switch
                            {
                                'W' => 0b00, 'E' => 0b01, 'N' => 0b10, 'S' => 0b11
                            };
                            int index = 0;
                            // Bytes 0-8:
                            // 8 bits reserved
                            // 11 bits identification
                            // 5+6 bits update time (hours:minutes)
                            // 7*6 bits countdowns
                            // Total 72 bits
                            message.Write(0, 8, ref index);
                            message.Write(MAGIC, 11, ref index);
                            message.Write((ulong)update.Hour, 5, ref index);
                            message.Write((ulong)update.Minute, 6, ref index);
                            foreach (byte countdown in countdowns)
                                message.Write(countdown, 7, ref index);
                            // Bytes 9-15:
                            // 6 bits schedule status
                            // 2*6 bits cancellation status
                            // 2*6 bits added stop/route
                            // 2 bits direction
                            // 10 bits route number
                            // 14 bits stop number (-32768-16384)
                            // Total 56 bits
                            foreach (byte status in schedule_stat)
                                message.Write(status, 1, ref index);
                            foreach (byte cancellation in cancellations)
                                message.Write(cancellation, 2, ref index);
                            foreach (byte addition in additions)
                                message.Write(addition, 2, ref index);
                            message.Write(direction, 2, ref index);
                            message.Write(route_id, 10, ref index);
                            message.Write(UInt64.Parse(stops[match][i_s_stop]) - 49152, 14, ref index);
                            Console.WriteLine("Broadcasting the following data:");
                            Console.WriteLine($"Last updated at {update.Hour:D2}:{update.Minute:D2}");
                            for (int i = 0; i < 6 && countdowns[i] != 255; i++)
                                Console.WriteLine($"Bus {i} in {countdowns[i]:D3} minutes, " +
                                    $"status: {(schedule_stat[i] == 1 ? "real-time" : "scheduled")}, " +
                                    $"cancelled: {cancellations[i]:B2}, added: {additions[i]}");
                            Console.WriteLine($"Route number: {route_name} (broadcasted: {route_id})");
                            Console.WriteLine($"Stop ID: {stops[match][i_s_stop]} (broadcasted: {UInt16.Parse(stops[match][i_s_stop]) - 49152})");
                            Bitfield encoded = message.ToHammingCode();
                            using UdpClient broadcast = new();
                            broadcast.Send(encoded.Bytes, encoded.Length, "255.255.255.255", MAGIC);
                        } while (String.IsNullOrEmpty(route) && end != json.Length);
                        Console.WriteLine("Done");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static bool IsCurrentCalendar(string gtfs_trip, string? route)
        {
            string gtfs_service = "";
            for (int i = 0; i < trips.Count; i++)
                if (trips[i][i_t_trip].Equals(gtfs_trip) && (String.IsNullOrEmpty(route) || trips[i][i_t_name].StartsWith($"{route} ")))
                    gtfs_service = trips[i][i_t_svc];
            if (String.IsNullOrEmpty(gtfs_service))
                return false;
            DateTime today = DateTime.Now;
            if (today.Hour < 5)
                today = today.AddDays(-1);
            string[] exception = new string[3];
            exception[i_cd_svc] = gtfs_service;
            exception[i_cd_date] = $"{today.Year:D4}{today.Month:D2}{today.Day:D2}";
            exception[i_cd_excep] = "1";
            for (int i = 0; i < calendar.Count; i++)
                if (calendar[i][i_c_svc].Equals(gtfs_service)
                    && (calendar[i][DateTime.Now.DayOfWeek == 0 ? i_c_week + 6 : ((int)DateTime.Now.DayOfWeek + i_c_week - 1)].Equals("1")
                        || calendar[i][i_c_week..(i_c_week + 6)].Equals(new string[] { "0", "0", "0", "0", "0", "0", "0" })
                        || dates.IndexOf(exception) != -1))
                    return true;
            return false;
        }

        private static DateTime Time(string time)
        {
            if (Int32.Parse(time.Split(':')[0]) < 24)
                return new DateTime(DateOnly.FromDateTime(DateTime.Now),
                    new TimeOnly(Int32.Parse(time.Split(':')[0]), Int32.Parse(time.Split(':')[1]), Int32.Parse(time.Split(':')[2])));
            else if (DateTime.Now.Hour < 9)
                return new DateTime(DateOnly.FromDateTime(DateTime.Now),
                    new TimeOnly(Int32.Parse(time.Split(':')[0]) - 24, Int32.Parse(time.Split(':')[1]), Int32.Parse(time.Split(':')[2])));
            else
                return new DateTime(DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    new TimeOnly(Int32.Parse(time.Split(':')[0]) - 24, Int32.Parse(time.Split(':')[1]), Int32.Parse(time.Split(':')[2])));
        }

        private static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
