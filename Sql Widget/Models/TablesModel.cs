using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql_Widget.Models
{
	public class TablesModel
	{
		private bool _subscribedToEvent;
		private Dictionary<string, List<string>> _cachedTables = new Dictionary<string, List<string>>();

		public Task<List<string>> GetAllTables(string dbName) => Task.Run(() => GetFromCache(dbName));

		private List<string> GetFromCache(string dbName)
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
			}

			if (!_subscribedToEvent)
			{
				DBModel.DBInvalidated += (s, e) => InvalidateCache();
				_subscribedToEvent = true;
			}
			return _cachedTables[dbName];
		}

		public void InvalidateCache() => _cachedTables = new Dictionary<string, List<string>>();
	}
}
