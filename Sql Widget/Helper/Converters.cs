using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Sql_Widget.Helper
{
	public class ResultToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((bool)value)
			{
				case true:
					return Brushes.ForestGreen;
				case false:
					return Brushes.Red;
			}
			return Brushes.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is SolidColorBrush)
			{
				if ((SolidColorBrush)value == Brushes.ForestGreen)
					return true;
				else
					return false;
			}
			return false;
		}
	}
}
