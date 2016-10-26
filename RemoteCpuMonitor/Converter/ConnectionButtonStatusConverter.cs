using RemoteCpuMonitor.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteCpuMonitor.Converter
{
    [ValueConversion(typeof(object), typeof(string))]
    public class ConnectionButtonStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "Connect";
            if (value != null)
            {
                SshClientStatusMessageType data = (SshClientStatusMessageType)value;
                switch (data)
                {
                    case SshClientStatusMessageType.Connecting:
                        result = "Connect";
                        break;
                    case SshClientStatusMessageType.Connected:
                        result = "Disconnect";
                        break;
                    case SshClientStatusMessageType.Disconnecting:
                        result = "Disconnect";
                        break;
                    case SshClientStatusMessageType.Disconnected:
                        result = "Connect";
                        break;
                    case SshClientStatusMessageType.ConnectionError:
                        result = "Connect";
                        break;
                    default:
                        result = "Connect";
                        break;
                }

            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
