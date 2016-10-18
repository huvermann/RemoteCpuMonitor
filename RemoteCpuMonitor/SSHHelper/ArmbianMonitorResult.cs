using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.SSHHelper
{
    public class ArmbianMonitorResult
    {
        public DateTime Time { get; set; }
        public int CpuSpeed { get; set; }
        public double Load { get; set; }
        public int Cpu { get; set; }
        public int Sys { get; set; }
        public int Usr { get; set; }
        public int Nice { get; set; }
        public int Io { get; set; }
        public int Irq { get; set; }
        public double Temperature { get; set; }
        static public ArmbianMonitorResult ParseMonitorString(string rawData)
        {
            ArmbianMonitorResult result = null;
            string pattern = @"(\d{2}:\d{2}:\d{2}):[\s]+(\d+)MHz[\s]+(\d+.\d+)[^\d]+(\d+)%[^\d]+(\d+)%[^\d]+(\d+)%[^\d]+(\d+)%[^\d]+(\d+)%[^\d]+(\d+)%[^\d]+(\d+)°C";
            MatchCollection match = Regex.Matches(rawData, pattern);
            if (match.Count == 1)
            {
                result = new ArmbianMonitorResult();
                result.Time = DateTime.ParseExact(match[0].Groups[1].Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                result.CpuSpeed = int.Parse(match[0].Groups[2].Value, CultureInfo.InvariantCulture);
                result.Load = double.Parse(match[0].Groups[3].Value, CultureInfo.InvariantCulture);
                result.Cpu = int.Parse(match[0].Groups[4].Value, CultureInfo.InvariantCulture);
                result.Sys = int.Parse(match[0].Groups[5].Value, CultureInfo.InvariantCulture);
                result.Usr = int.Parse(match[0].Groups[6].Value, CultureInfo.InvariantCulture);
                result.Nice = int.Parse(match[0].Groups[7].Value, CultureInfo.InvariantCulture);
                result.Io = int.Parse(match[0].Groups[8].Value, CultureInfo.InvariantCulture);
                result.Irq = int.Parse(match[0].Groups[9].Value, CultureInfo.InvariantCulture);
                result.Temperature = double.Parse(match[0].Groups[10].Value, CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}
