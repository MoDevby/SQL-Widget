using Sql_Widget.Helper;
using Sql_Widget.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sql_Widget.ViewModels
{
	class ResultVM : INotifyPropertyChanged
	{
#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		#region Properties
		private readonly string _dbName;
		private int ResultCount => (Result == null ? 0 : Result.Count);
		public string Title => ($"({DateTime.Now.ToShortTimeString()}) DB:{_dbName} [{ResultCount} Rows]");
		public DataView Result { get; set; }
		public string Query { get; set; }
		public Visibility Loadding => Result == null ? Visibility.Visible : Visibility.Collapsed;
		public Visibility NotLoadding => Result == null ? Visibility.Collapsed : Visibility.Visible;
		#endregion

		#region Methods

		public ResultVM(string dbName, string query)
		{
			_dbName = dbName;
			Query = query.Replace(Environment.NewLine, " ");
			PopulateResult(dbName, query);
		}

		private async void PopulateResult(string dbName, string query)
		{
			var result = await Task.Run(() => new QueryModel().Execute(dbName, query));
			Result = result.DefaultView;
		}

		public Tuple<bool, int> GetResultInfo()
		{
			while (Result == null)
				Thread.Sleep(100);
			var succeed = Result.Table.Columns.Count == 0 || Result.Table.Columns[0].ColumnName != "Errors";
			return new Tuple<bool, int>(succeed, ResultCount);
		}
		#endregion

		#region Commands
		public ICommand Execute
		{
			get
			{
				return new ButtonsCommand((object obj) =>
				{
					Result = null;
					PopulateResult(_dbName, Query);
				});
			}
		}
		#endregion
	}
}
