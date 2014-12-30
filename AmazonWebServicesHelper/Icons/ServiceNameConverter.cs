using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace AmazonWebServicesHelper.Icons
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ServiceNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ServiceCategory cat = (ServiceCategory)value;
            return cat.ToString().Replace("_"," & ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
