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
    [ValueConversion(typeof(SshClientStatusMessageType), typeof(bool))]
    public class DisableWhileOnlineTransitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (value != null)
            {
                SshClientStatusMessageType data = (SshClientStatusMessageType)value;
                switch (data)
                {
                    case SshClientStatusMessageType.Connecting:
                        result = false;
                        break;
                    case SshClientStatusMessageType.Connected:
                        result = true;
                        break;
                    case SshClientStatusMessageType.Disconnecting:
                        result = false;

                        break;
                    case SshClientStatusMessageType.Disconnected:
                        result = true;
                        break;
                    case SshClientStatusMessageType.ConnectionError:
                        result = true;
                        break;
                    default:
                        result = false;
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
