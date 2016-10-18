using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace RemoteCpuMonitor.SSHHelper
{
    public class CpuTempMonitorMessage
    {
        public DateTime Time { get; set; }
        public double Temperature1 { get; set; }
        public double Temperature2 { get; set; }
        public int CpuSpeed1 { get; set; }
        public int CpuSpeed2 { get; set; }
        public int CpuSpeed3 { get; set; }
        public int CpuSpeed4 { get; set; }

        static public CpuTempMonitorMessage ParseMonitorString(string rawData)
        {
            CpuTempMonitorMessage result = null;
            string pattern = @"(\d{2}:\d{2}:\d{2}) (\d+)°C (\d+)°C (\d+) MHz (\d+) MHz (\d+) MHz (\d+) MHz";
            MatchCollection match = Regex.Matches(rawData, pattern);
            if (match.Count == 1)
            {
                result = new CpuTempMonitorMessage();
                result.Time = DateTime.ParseExact(match[0].Groups[1].Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                result.Temperature1 = double.Parse(match[0].Groups[2].Value, CultureInfo.InstalledUICulture);
                result.Temperature2 = double.Parse(match[0].Groups[3].Value, CultureInfo.InstalledUICulture);
                result.CpuSpeed1 = int.Parse(match[0].Groups[4].Value, CultureInfo.InstalledUICulture);
                result.CpuSpeed2 = int.Parse(match[0].Groups[5].Value, CultureInfo.InstalledUICulture);
                result.CpuSpeed3 = int.Parse(match[0].Groups[6].Value, CultureInfo.InstalledUICulture);
                result.CpuSpeed4 = int.Parse(match[0].Groups[7].Value, CultureInfo.InstalledUICulture);
            }
            return result;
        }
    }
}
