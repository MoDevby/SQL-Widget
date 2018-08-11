using System;
using System.Data;
using System.Data.SqlClient;

namespace Sql_Widget.Models
{
	class QueryModel
	{
		public static string ConnectionString(string dbName) => $"server=.;Database={dbName};Integrated Security=true";

		//public Task<DataTable> ExecuteQuery(string dbName, string query) => Task.Run(() => Execute(dbName, query));

		public DataTable Execute(string dbName, string query)
		{
			using (SqlConnection con = new SqlConnection(ConnectionString(dbName)))
			{
				//Thread.Sleep(10000);
				SqlCommand cmd = new SqlCommand(query, con);
				SqlDataAdapter sda = new SqlDataAdapter(cmd);
				DataTable dt = new DataTable();
				try
				{ sda.Fill(dt); }
				catch (Exception ex)
				{
					dt.Columns.Add(new DataColumn("Errors"));
					dt.Rows.Add(ex.Message);
					if (ex.InnerException != null)
						dt.Rows.Add(ex.InnerException.Message);
				}

				return dt;
			}
		}
	}
}
