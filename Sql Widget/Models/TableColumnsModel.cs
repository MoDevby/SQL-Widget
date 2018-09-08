using Sql_Widget.Classes;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql_Widget.Models
{
	public class TableColumnsModel
	{
		private static bool _subscribedToEvent;

		private static Dictionary<string, List<TableColumn>> _cachedColumns = new Dictionary<string, List<TableColumn>>();

		public static Task<List<TableColumn>> GetTableColumns(string db, string table) => Task.Run(() => GetFromCache(db, table));

		private static string _selectAllTableColumns(string table) =>
					"SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,IS_NULLABLE " +
					$" FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' ORDER BY COLUMN_NAME";

		private static List<TableColumn> GetFromCache(string db, string table)
		{
			if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(db))
				return new List<TableColumn>();
			if (!_cachedColumns.ContainsKey(table))
			{
				List<TableColumn> columns = new List<TableColumn>();
				using (SqlConnection con = new SqlConnection(QueryModel.ConnectionString(db)))
				{
					con.Open();
					using (SqlCommand cmd = new SqlCommand(_selectAllTableColumns(table), con))
					{
						using (IDataReader dr = cmd.ExecuteReader())
							while (dr.Read())
								columns.Add(new TableColumn
								{
									Position = dr.GetInt32(0),
									Name = dr.GetString(1),
									Type = dr.GetString(2),
									MaxLength = dr[3] as int?,
									Precision = dr[4] as byte?,
									IsNullable = dr.GetString(5) == "YES"
								});
					}
				}
				_cachedColumns.Add(table, columns);
			}
			if (!_subscribedToEvent)
			{
				DBModel.DBInvalidated += (s, e) => InvalidateCache();
				_subscribedToEvent = true;
			}
			return _cachedColumns[table];
		}

		public static void InvalidateCache() => _cachedColumns = new Dictionary<string, List<TableColumn>>();
	}
}
