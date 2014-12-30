using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace AmazonWebServicesHelper.Icons
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ServiceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ServiceCategory cat = (ServiceCategory)value;
            int val = (int)cat;

            return new SolidColorBrush(Color.FromRgb((byte)(val >> 16), (byte)(val >> 8), (byte)(val >> 0)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
