using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sql_Widget.Models
{
	public class DBModel
	{
		private const string _dbName = "master";
		private const string _selectAllDBsString =
			"SELECT name FROM sys.databases WHERE name NOT IN ('master', 'model', 'tempdb', 'msdb')";

		private List<string> _cachedDBs = new List<string>();

		public Task<List<string>> GetAllDBs() => Task.Run(() => GetDbs());

		private List<string> GetDbs()
		{
			if (!_cachedDBs.Any())
				using (SqlConnection con = new SqlConnection(QueryModel.ConnectionString(_dbName)))
				{
					con.Open();
					using (SqlCommand cmd = new SqlCommand(_selectAllDBsString, con))
					{
						using (IDataReader dr = cmd.ExecuteReader())
							while (dr.Read())
								_cachedDBs.Add(dr[0].ToString());
					}
				}
			return _cachedDBs;
		}

		public void InvalidateCache() { _cachedDBs = new List<string>(); DBInvalidated(null, EventArgs.Empty); }
		public static event EventHandler DBInvalidated = delegate { };

	}
}
