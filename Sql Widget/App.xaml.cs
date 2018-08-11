using Sql_Widget.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Sql_Widget
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//This is used to load the assembly from the resources instead of having it as standalone dll
			AppDomain.CurrentDomain.AssemblyResolve += (se, args) =>
			{
				Assembly thisAssembly = Assembly.GetExecutingAssembly();

				//Get the Name of the AssemblyFile
				var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

				//Load form Embedded Resources - This Function is not called if the Assembly is in the Application Folder
				var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(name));
				if (resources.Any())
				{
					var resourceName = resources.First();
					using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
					{
						if (stream == null) return null;
						var block = new byte[stream.Length];
						stream.Read(block, 0, block.Length);
						return Assembly.Load(block);
					}
				}
				return null;
			};

			// Force first control to have focus
			EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent,
				new RoutedEventHandler(delegate (object s, RoutedEventArgs args)
				{
					((Window)s).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}));

			// Start the main window
			var main = new MainWindow();
			main.DataContext = new MainWindowVM();
			main.Show();
		}

	}
}
