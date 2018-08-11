using Sql_Widget.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sql_Widget.ViewModels
{
	class ResultVM : INotifyPropertyChanged
	{
#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		public string Title { get; set; }
		public DataView Result { get; set; }
		public Visibility Loadding => Result == null ? Visibility.Visible : Visibility.Collapsed;
		public Visibility NotLoadding => Result == null ? Visibility.Collapsed : Visibility.Visible;

		public ResultVM(string dbName, string query)
		{
			Title = $"({DateTime.Now.ToString("HH:mm:ss")}) {query}";
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
			var succeed = Result.Table.Columns[0].ColumnName != "Errors";
			var rowCount = Result.Count;
			return new Tuple<bool, int>(succeed, rowCount);
		}
	}
}
