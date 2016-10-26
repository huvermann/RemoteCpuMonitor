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
    public class OnlineStatusStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "Offline";

            if (value != null)
            {
                SshClientStatusMessageType data = (SshClientStatusMessageType)value;
                switch (data)
                {
                    case SshClientStatusMessageType.Connecting:
                        result = "Connecting...";
                        break;
                    case SshClientStatusMessageType.Connected:
                        result = "Connected";
                        break;
                    case SshClientStatusMessageType.Disconnecting:
                        result = "Disconnectiong...";
                        break;
                    case SshClientStatusMessageType.Disconnected:
                        result = "Offline";
                        break;
                    case SshClientStatusMessageType.ConnectionError:
                        result = "Error!";
                        break;
                    default:
                        break;
                }

            } else
            {
                return "Offline";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
