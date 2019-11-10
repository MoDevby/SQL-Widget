using Sql_Widget.Entities;
using Sql_Widget.Helper;
using Sql_Widget.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sql_Widget.ViewModels
{
	class MainWindowVM : INotifyPropertyChanged
	{
#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
		private DBModel _dbModel = new DBModel();

		#region Properties
		#region Window
		public TabItem SelectedTab { get; set; }
		public bool TopMost { get; set; }
		public bool VisibleInTaskbar { get { return !TopMost; } }
		#endregion
		#region Server
		private bool _useWinAuth;
		public bool ValidConnection { get; set; }
		public string ServerName { get; set; }
		public string ServerMessage { get; set; }
		public bool UseWinAuth
		{
			get { return _useWinAuth; }
			set
			{
				_useWinAuth = value;
				if (!value) return;

				UserName = "";
			}
		}
		public string UserName { get; set; }
		#endregion
		#region DB
		public string SelectedDB { get; set; }
		public List<string> DBsList { get; set; } = new List<string>();
		#endregion
		#region Query
		public string QueryContent { get; set; }
		public string ExecutableQuery { get; set; }
		public bool CanExecute => ValidConnection && !string.IsNullOrWhiteSpace(SelectedDB) &&
				(!string.IsNullOrWhiteSpace(QueryContent) || SelectedTab.Header.ToString() != "Query");
		public bool IsValidExecutableQuery => ValidConnection &&
			!string.IsNullOrWhiteSpace(SelectedDB) && !string.IsNullOrWhiteSpace(ExecutableQuery);
		#endregion
		#region History
		public List<HistoryItem> HistoryItems { get; set; } = new List<HistoryItem>();
		#endregion
		#endregion

		#region Methods
		public MainWindowVM()
		{
			VerfiyServer();
		}

		private async void VerfiyServer()
		{
			ServerName = Properties.Settings.Default.Server;
			UserName = Properties.Settings.Default.UserName;

			ServerMessage = "Connecting....";

			if (await _dbModel.CheckConnection())
			{
				ValidConnection = true;
				ServerMessage = "Connected!";
				AsyncLoadDBs();
			}
			else
			{
				ServerMessage = "Error, no connection was established to the server!";
				ValidConnection = false;
			}
		}

		private async void AsyncLoadDBs()
		{
			SelectedDB = null;
			_dbModel.InvalidateCache();
			try
			{
				DBsList = await _dbModel.GetAllDBs();
			}
			catch (Exception)
			{
				DBsList = new List<string>();
				ServerMessage = $"Error, Couldn't Load The DBs from {ServerName}!";
				ValidConnection = false;
			}
		}

		private async void AddToHistory(string db, string query, ResultVM resultVm)
		{
			var historyItem = new HistoryItem
			{
				Date = $"({DateTime.Now.ToShortTimeString()})",
				DBName = db,
				Query = query
			};
			var result = await Task.Run(() => resultVm.GetResultInfo());
			historyItem.Succeeded = result.Item1;
			historyItem.RowCount = result.Item2;

			HistoryItems.Add(historyItem);
			HistoryItems = new List<HistoryItem>(HistoryItems);
		}
		#endregion

		#region Commands
		#region Window
		public ICommand ChangeTab
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var tabs = (TabControl)obj;
					tabs.SelectedIndex = tabs.SelectedIndex < tabs.Items.Count - 1 ? tabs.SelectedIndex + 1 : 0;
				});
			}
		}
		#endregion
		#region Server
		public ICommand SaveServerCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					ValidConnection = false;
					Properties.Settings.Default.Server = ServerName;
					Properties.Settings.Default.UserName = UserName;
					Properties.Settings.Default.Password = (obj as PasswordBox).Password;
					Properties.Settings.Default.Save();
					VerfiyServer();
				});
			}
		}
		#endregion
		#region DB Bar

		public ICommand HelpCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					Process.Start("Help.md");
				});
			}
		}
		public ICommand CloseCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var window = (Window)obj;
					window.Close();
					if (Application.Current == null) return;
					Application.Current.Shutdown();
					//Environment.Exit(1);
				});
			}
		}
		public ICommand SelectDBCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var comboBox = (ComboBox)obj;

					if (!DBsList.Contains(comboBox.Text))
						comboBox.Text = "";
					SelectedDB = comboBox.Text;
				});
			}
		}
		#endregion
		#region Favorite
		public ICommand AddFavoriteCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					FavoriteModel.AddNewFavoriteItem(new FavoriteItem() { Description = "Hi motherfuckers", Query = "Select * from @VAR_1  @VAR_2" });
					//todo: show favorite window without index
					FavoriteItems = FavoriteModel.GetFavoriteList();
				});
			}
		}

		public ICommand EditFavoriteCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var item = (FavoriteItem)obj;
					//todo: show favorite window and pass the index
					FavoriteItems = FavoriteModel.GetFavoriteList();
				});
			}
		}
		#endregion
		#region Bottom Buttons
		public ICommand ExecuteCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					switch (SelectedTab.Header.ToString())
					{
						case "Query":
							ExecutableQuery = QueryContent;
							break;
						case "History":
							ExecutableQuery = string.Join(";", HistoryItems.Where(x => x.Selected).Select(x => x.Query));
							break;
						case "Favorite":
							ExecutableQuery = string.Join(";", FavoriteItems.Where(x => x.Selected).Select(x => x.QueryWithVariables));
							break;
						default:
							break;
					}
					if (!IsValidExecutableQuery) return;
					foreach (string query in ExecutableQuery.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)))
					{
						var newResult = new ResultWindow();
						var vm = new ResultVM(SelectedDB, query);
						newResult.DataContext = vm;
						newResult.Show();
						AddToHistory(SelectedDB, query, vm);
					}
				});
			}
		}

		public ICommand ClearCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					switch (SelectedTab.Header.ToString())
					{
						case "Query":
							QueryContent = string.Empty;
							break;
						case "History":
							HistoryItems.ForEach(x => x.Selected = false);
							HistoryItems = new List<HistoryItem>(HistoryItems);
							break;
						case "Favorite":
							FavoriteItems.ForEach(x => x.Selected = false);
							FavoriteItems = FavoriteModel.GetFavoriteList();
							break;
						default:
							break;
					}
				});
			}
		}
		#endregion
		#endregion
	}
}
