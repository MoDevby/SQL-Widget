using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql_Widget.Models
{
	public class TablesModel
	{
		private static Dictionary<string, List<string>> _cachedTables = new Dictionary<string, List<string>>();

		public static void InvalidateCache() => _cachedTables = new Dictionary<string, List<string>>();

		public static Task<List<string>> GetAllTables(string dbName) => Task.Run(() => GetTablesFromCache(dbName));

		private static List<string> GetTablesFromCache(string dbName)
		{
			if (string.IsNullOrWhiteSpace(dbName))
				return new List<string>();
			if (!_cachedTables.ContainsKey(dbName))
			{
				List<string> tables = new List<string>();
				using (SqlConnection con = new SqlConnection(QueryModel.ConnectionString(dbName)))
				{
					con.Open();
					DataTable schema = con.GetSchema("Tables");
					foreach (DataRow row in schema.Rows)
					{
						tables.Add(row[2].ToString());
					}
				}
				_cachedTables.Add(dbName, tables);
				_cachedTables[dbName].Sort((a, b) => a.CompareTo(b));
			}

			return _cachedTables[dbName];
		}
	}
}
