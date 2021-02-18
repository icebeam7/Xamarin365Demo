using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamarin365Demo.Converters
{
    public class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rating = (int)value;
            var star = int.Parse(parameter.ToString());

            return star <= rating ? "star_selected.png" : "star_outline.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}