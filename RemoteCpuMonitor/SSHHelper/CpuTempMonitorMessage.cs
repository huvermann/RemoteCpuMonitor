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
        public double Temperature { get; set; }
        public int CpuSpeed { get; set; }
        public double CpuLoad1 { get; set; }
        public double CpuLoad2 { get; set; }
        public double CpuLoad3 { get; set; }
        public double CpuLoad4 { get; set; }
        public object Sender { get; set; }

        static public CpuTempMonitorMessage ParseMonitorString(object Sender, string rawData)
        {
            CpuTempMonitorMessage result = null;
            string pattern = @"(\d{2}:\d{2}:\d{2})\s+(\d+[,.]\d+)[^\d]+(\d+)\sMHz\s(\d+[,.]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})";
            MatchCollection match = Regex.Matches(rawData, pattern);
            if (match.Count == 1)
            {
                result = new CpuTempMonitorMessage();
                result.Time = DateTime.ParseExact(match[0].Groups[1].Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                result.Temperature = double.Parse(match[0].Groups[2].Value, CultureInfo.InvariantCulture);
                result.CpuSpeed = int.Parse(match[0].Groups[3].Value, CultureInfo.InvariantCulture);
                result.CpuLoad1 = double.Parse(match[0].Groups[4].Value, CultureInfo.InvariantCulture);
                result.CpuLoad2 = double.Parse(match[0].Groups[5].Value, CultureInfo.InvariantCulture);
                result.CpuLoad3 = double.Parse(match[0].Groups[6].Value, CultureInfo.InvariantCulture);
                result.CpuLoad4 = double.Parse(match[0].Groups[7].Value, CultureInfo.InvariantCulture);
                result.Sender = Sender;
            }
            return result;
        }
    }
}
