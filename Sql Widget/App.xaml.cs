using Sql_Widget.ViewModels;
using Sql_Widget.Views;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Sql_Widget
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		//[System.Runtime.InteropServices.DllImport("user32.dll")]
		//private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		//private const int SW_SHOWMAXIMIZED = 3;
		private static Mutex _mutex = null;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//Allow only one instance of the application
			CheckMultipleInstances();

			//Force first control to have focus
			InitiateFocus();

			//Start the main window
			this.MainWindow = new MainWindow();
			this.MainWindow.DataContext = new MainWindowVM();
			this.MainWindow.Show();
		}

		private void CheckMultipleInstances()
		{
			//Process currentProcess = Process.GetCurrentProcess();
			//var runningProcess = (from process in Process.GetProcesses()
			//					  where
			//						process.Id != currentProcess.Id &&
			//						process.ProcessName.Equals(
			//						  currentProcess.ProcessName,
			//						  StringComparison.Ordinal)
			//					  select process).FirstOrDefault();
			//if (runningProcess != null)
			//{
			//	ShowWindow(runningProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
			//	Current.Shutdown();
			//}

			const string appName = "MySqlWidget";
			bool createdNew;

			_mutex = new Mutex(true, appName, out createdNew);

			if (!createdNew)
			{
				Current.Shutdown();
			}
		}

		private static void InitiateFocus()
		{
			EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent,
							new RoutedEventHandler(delegate (object s, RoutedEventArgs args)
							{
								((Window)s).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
							}));
		}
	}
}
