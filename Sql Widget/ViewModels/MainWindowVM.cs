using Sql_Widget.Classes;
using Sql_Widget.Helper;
using Sql_Widget.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private string[] _excludedFromReset = { "Query", "History" };
		private string _selectedDB;
		private string _selectedTable;
		private TablesModel _tableModel = new TablesModel();
		private DBModel _dbModel = new DBModel();
		private TabItem _selectedTab;
		#region Properties
		#region Window
		public TabItem SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				_selectedTab = value;
				if (!_excludedFromReset.Contains(value.Header.ToString()))
					ResetCommand.Execute("");
			}
		}
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
		#endregion
		#region Query
		public string QueryValue { get; set; }
		public bool QueryEnabled => !string.IsNullOrWhiteSpace(SelectedDB);
		private bool IsValidQuery => !string.IsNullOrWhiteSpace(SelectedDB) && !string.IsNullOrWhiteSpace(QueryValue);
		#endregion
		public List<TableColumn> Fields { get; set; } = new List<TableColumn>();
		public bool ComponentsEnabled => Fields.Any();
		#region Select
		public List<TableColumn> VisibleFields => Fields.Where(x => !SelectedFields.Contains(x)).ToList();
		public List<TableColumn> SelectedFields { get; set; } = new List<TableColumn>();
		private bool IsValidSelect => SelectedFields.Any();
		#endregion
		#region Where
		public List<ConditionElement> Conditions { get; set; } = new List<ConditionElement> { new ConditionElement() };
		public Dictionary<CompareOperators, string> CompareOperators { get; set; }
		public List<RelationOperators> RelationOperators { get; set; }
		#endregion
		#region History
		public List<HistoryItem> HistoryItems { get; set; } = new List<HistoryItem>();
		public HistoryItem SelectedHistoryElement { get; set; }
		#endregion
		#endregion

		#region Methods
		public MainWindowVM()
		{
			CompareOperators = OperatorsModel.GetCompareOperators();
			RelationOperators = OperatorsModel.GetReleationsOperators();

			AsyncLoadDBs();
			IntiateFields();
		}

		private async void IntiateFields()
		{
			SelectedFields = new List<TableColumn>();
			Conditions = new List<ConditionElement> { new ConditionElement() };
			Fields = await TableColumnsModel.GetTableColumns(SelectedDB, SelectedTable);
		}

		private async void AsyncLoadDBs() => DBsList = await _dbModel.GetAllDBs();

		private async void AsyncUpdateTables() => TablesList = string.IsNullOrWhiteSpace(_selectedDB)
			? new List<string>()
			: await _tableModel.GetAllTables(SelectedDB);

		private void PopulateSelectQuery()
		{
			if (!IsValidSelect) return;
			QueryValue = string.Empty;
			var fields = string.Join(", ", SelectedFields.Select(x => x.Name));
			QueryValue = $"SELECT {fields}\nFROM {SelectedTable}";

			var whereConsitions = Conditions.Where(x => x.SelectedField != "" && x.SelectedOerator != 0);
			QueryValue += ConstructWhereClause(whereConsitions.ToList());
		}

		private string ConstructWhereClause(List<ConditionElement> whereConsitions)
		{
			if (!whereConsitions.Any()) return null;
			var where = "\nWhere ";
			var index = 0;
			whereConsitions.ForEach(con =>
			{
				where += $"{con.SelectedField} {CompareOperators.FirstOrDefault(x => x.Key == con.SelectedOerator).Value} ";
				where += IsStringType(con.SelectedField) && string.IsNullOrWhiteSpace(con.Value) && con.Value != "null"
							? $"'{con.Value}'"
							: string.IsNullOrWhiteSpace(con.Value) ? "null" : con.Value;
				if (index < whereConsitions.Count() - 1)
					where += con.NextLineOperator == 0 ? $" {RelationOperators.First()} " : $" {con.NextLineOperator} ";
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
		#region DB Bar
		public ICommand ResetCacheCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					_dbModel.InvalidateCache();
					AsyncLoadDBs();
					SelectedDB = null;
				});
			}
		}
		public ICommand HelpCommand
		{
			get
			{
				return null;
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
						//Fields.Remove(item);
					}
					SelectedFields = new List<TableColumn>(SelectedFields);
					//Fields = new List<TableColumn>(Fields);
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
						//Fields.Add(item);
						SelectedFields.Remove(item);
					}
					//Fields = new List<TableColumn>(Fields);
					SelectedFields = new List<TableColumn>(SelectedFields);
				});
			}
		}
		#endregion
		#region Where
		//[AlsoNotifyFor("Conditions")]
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
							break;
						case "SELECT":
							PopulateSelectQuery();
							break;
						case "History":
							if (SelectedHistoryElement == null) return;
							QueryValue = SelectedHistoryElement.Query;
							break;
						default:
							break;
					}
					if (!IsValidQuery) return;
					var newResult = new ResultWindow();
					var vm = new ResultVM(SelectedDB, QueryValue);
					newResult.DataContext = vm;
					newResult.Show();
					AddToHistory(SelectedDB, QueryValue, vm);
				});
			}
		}

		public ICommand ResetCommand
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					SelectedFields = new List<TableColumn>();
					Conditions = new List<ConditionElement> { new ConditionElement() };
					QueryValue = string.Empty;
				});
			}
		}
		#endregion
		#endregion


	}
}
