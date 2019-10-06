using System.Windows;

namespace Sql_Widget
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
			this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
			//Expander_Collapsed("", new RoutedEventArgs());
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 100)
				this.SizeToContent = SizeToContent.WidthAndHeight;
			else
			{
				this.SizeToContent = SizeToContent.Width;
				this.Height = 135;
			}

			this.Left = SystemParameters.PrimaryScreenWidth - e.NewSize.Width;
			this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (e.NewSize.Height / 2);
		}

		//private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		//{
		//	if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
		//		this.DragMove();
		//}
	}
}
