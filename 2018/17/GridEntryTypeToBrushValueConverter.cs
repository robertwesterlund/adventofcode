using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace _17
{
    public class GridEntryTypeToBrushValueConverter : IValueConverter
    {
        private static readonly Dictionary<GridEntryType, Brush> _brushes = new Dictionary<GridEntryType, Brush>()
        {
            [GridEntryType.Sand] = new SolidColorBrush(Colors.Beige),
            [GridEntryType.Clay] = new SolidColorBrush(Colors.SandyBrown),
            [GridEntryType.RunningWater] = new SolidColorBrush(Colors.LightBlue),
            [GridEntryType.StillWater] = new SolidColorBrush(Colors.DarkBlue)
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridEntryType t)
            {
                return _brushes[t];
            }
            else
            {
                throw new Exception("Could not convert object, don't care...");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
