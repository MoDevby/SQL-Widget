using Sql_Widget.Entities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sql_Widget.Helper
{
	[ValueConversion(typeof(bool), typeof(Brushes))]
	public class ResultToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//	if (value is SolidColorBrush)
			//	{
			//		if ((SolidColorBrush)value == Brushes.ForestGreen)
			//			return true;
			//		else
			//			return false;
			//	}
			//	return false;
			throw new NotImplementedException();
		}
	}

	public class VariableVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var varName = (VariablesNames)Enum.Parse(typeof(VariablesNames), parameter.ToString());
			return (!string.IsNullOrWhiteSpace(value.ToString()) && value.ToString().Contains('@' + varName.ToString()))
				? Visibility.Visible
				: Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new InvalidOperationException("The target must be a boolean");

			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
