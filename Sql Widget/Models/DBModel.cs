using Sql_Widget.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sql_Widget.Models
{
	public class DBModel
	{
		private const string _dbName = "master";
		private const string _selectAllDBsString = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'model', 'tempdb', 'msdb')";
		private List<string> _cachedDBs = new List<string>();

		public void InvalidateCache()
		{
			_cachedDBs = new List<string>();
			TablesModel.InvalidateCache();
			TableColumnsModel.InvalidateCache();
		}

		public async Task<List<string>> GetAllDBs()
		{
			if (!_cachedDBs.Any())
				using (SqlConnection con = new SqlConnection(QueryModel.GetConnectionString(_dbName)))
				{
					await con.OpenAsync();
					using (SqlCommand cmd = new SqlCommand(_selectAllDBsString, con))
					{
						using (var dr = await cmd.ExecuteReaderAsync())
							while (await dr.ReadAsync())
								_cachedDBs.Add(await dr.GetTextReader(0).ReadToEndAsync());
					}
				}
			return _cachedDBs;
		}

		public async Task<bool> CheckConnection()
		{
			using (SqlConnection con = new SqlConnection(QueryModel.GetConnectionString(_dbName)))
			{
				try
				{
					await con.OpenAsync().TimeoutAfter(10000);
					con.Close();
					return true;
				}
				catch(Exception ex)
				{
					return false;
				}
			}
		}
	}
}
