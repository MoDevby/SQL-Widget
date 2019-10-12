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
		private string _selectedDB;
		private string _selectedTable;
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
		public List<string> DBsList { get; set; } = new List<string>();
		public List<string> TablesList { get; set; } = new List<string>();
		public string SelectedDB
		{
			get { return _selectedDB; }
			set
			{
				_selectedDB = value;
				AsyncUpdateTables();
				SelectedTable = null;
			}
		}
		public string SelectedTable
		{
			get { return _selectedTable; }
			set { _selectedTable = value; IntiateFields(); }
		}
		public bool SelectTablesIsEnabled => TablesList.Any();
		public List<TableColumn> Fields { get; set; } = new List<TableColumn>();
		public bool ComponentsEnabled => Fields.Count() > 1;
		#endregion
		#region Query
		public string QueryContent { get; set; }
		public string ExecutableQuery { get; set; }
		public bool QueryEnabled => ValidConnection && !string.IsNullOrWhiteSpace(SelectedDB);
		private bool IsValidExecutableQuery => !string.IsNullOrWhiteSpace(SelectedDB) && !string.IsNullOrWhiteSpace(ExecutableQuery);
		#endregion
		#region Select
		public List<TableColumn> VisibleFields => Fields.Where(x => !SelectedFields.Contains(x)).ToList();
		public List<TableColumn> SelectedFields { get; set; } = new List<TableColumn>();
		private bool IsValidSelect => SelectedFields.Any();
		#endregion
		#region Where
		public List<TableColumn> WhereFields { get { return Fields.Where(a => a.Name != "*").ToList(); } }
		public List<ConditionElement> Conditions { get; set; } = new List<ConditionElement> { new ConditionElement() };
		public Dictionary<CompareOperators, string> CompareOperators { get; set; }
		public List<RelationOperators> RelationOperators { get; set; }
		#endregion
		#region History
		public List<HistoryItem> HistoryItems { get; set; } = new List<HistoryItem>();
		#endregion
		#endregion

		#region Methods
		public MainWindowVM()
		{
			CompareOperators = OperatorsModel.GetCompareOperators();
			RelationOperators = OperatorsModel.GetReleationsOperators();

			VerfiyServer();
		}

		private async void IntiateFields()
		{
			SelectedFields = new List<TableColumn>();
			Conditions = new List<ConditionElement> { new ConditionElement() };
			Fields = new List<TableColumn> { new TableColumn { Name = "*", Position = 0, Type = "*" } }
				.Concat(await TableColumnsModel.GetTableColumns(SelectedDB, SelectedTable)).ToList();
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
				IntiateFields();
			}
			else
			{
				ServerMessage = "Error, no connection was established to the server!";
				ValidConnection = false;
			}
		}

		private async void AsyncLoadDBs()
		{
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

		private async void AsyncUpdateTables()
		{
			if (!string.IsNullOrWhiteSpace(_selectedDB))
				try
				{
					TablesList = await TablesModel.GetAllTables(SelectedDB);
					return;
				}
				catch (Exception)
				{
					ServerMessage = $"Error, Couldn't Load The Tables for {SelectedDB}!";
					ValidConnection = false;
				}
			TablesList = new List<string>();
		}

		private string GetSelectQuery()
		{
			var Query = string.Empty;
			if (IsValidSelect)
			{
				var fields = string.Join(", ", SelectedFields.Select(x => x.Name));
				Query = $"SELECT {fields}{Environment.NewLine}FROM {SelectedTable}";

				var whereConditions = Conditions.Where(x => x.SelectedField != "" && x.SelectedOerator != 0);
				Query += ConstructWhereClause(whereConditions);
			}
			return Query;
		}

		private string ConstructWhereClause(IEnumerable<ConditionElement> whereConditions)
		{
			if (!whereConditions.Any()) return null;
			var where = Environment.NewLine + "Where ";
			var index = 0;
			whereConditions.ToList().ForEach(condition =>
			{
				where += condition.SelectedField + CompareOperators.First(x => x.Key == condition.SelectedOerator).Value;
				where += IsStringType(condition.SelectedField) && condition.Value != "null"
							? string.IsNullOrEmpty(condition.Value) ? "" : $"'{condition.Value}'"
							: string.IsNullOrWhiteSpace(condition.Value) ? "null" : condition.Value;
				if (index < whereConditions.Count() - 1)
					where += condition.NextLineOperator == 0 ? $" {RelationOperators.First()} " : $" {condition.NextLineOperator} ";
				index++;
			});
			return where;
		}
		private bool IsStringType(string fieldName)
		{
			var type = Fields.FirstOrDefault(x => x.Name == fieldName).Type;
			return type.Contains("char") || type.Contains("uniqueidentifier") || type.Contains("date");
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
					ResetCacheCommand.Execute("");
				});
			}
		}
		#endregion
		#region DB Bar
		public ICommand ResetCacheCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					SelectedDB = null;
					_dbModel.InvalidateCache();
					VerfiyServer();
				});
			}
		}
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
					var db = (string)obj;
					SelectedDB = db;
				});
			}
		}
		#endregion
		#region Select
		public ICommand AddFieldCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var list = (ListBox)obj;
					foreach (TableColumn item in list.SelectedItems)
					{
						SelectedFields.Add(item);
					}
					SelectedFields = new List<TableColumn>(SelectedFields);
				});
			}
		}
		public ICommand RemoveFieldCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					var list = (ListBox)obj;
					foreach (TableColumn item in list.SelectedItems)
					{
						SelectedFields.Remove(item);
					}
					SelectedFields = new List<TableColumn>(SelectedFields);
				});
			}
		}
		#endregion
		#region Where
		public ICommand AddConditionCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					Conditions.Add(new ConditionElement());
					Conditions = new List<ConditionElement>(Conditions);
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
						case "SELECT":
							ExecutableQuery = GetSelectQuery();
							break;
						case "History":
							ExecutableQuery = string.Join(";", HistoryItems.Where(x => x.Selected).Select(x => x.Query));
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
						case "SELECT":
							SelectedFields = new List<TableColumn>();
							Conditions = new List<ConditionElement> { new ConditionElement() };
							break;
						case "History":
							HistoryItems.ForEach(x => x.Selected = false);
							HistoryItems = new List<HistoryItem>(HistoryItems);
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
