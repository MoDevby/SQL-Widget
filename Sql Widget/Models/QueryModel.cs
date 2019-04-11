using System;
using System.Data;
using System.Data.SqlClient;

namespace Sql_Widget.Models
{
	class QueryModel
	{
		public static string ConnectionString(string dbName) => $"server=.;Database={dbName};Integrated Security=true";

		public DataTable Execute(string dbName, string query)
		{
			using (SqlConnection con = new SqlConnection(ConnectionString(dbName)))
			{
				//Thread.Sleep(1000);
				DataTable dt = new DataTable();
				try
				{
					switch (query.Split(' ')[0].ToLower())
					{
						case "select":
							var sda = new SqlDataAdapter(query, con);
							sda.Fill(dt);
							break;
						default:
							con.Open();
							var rowCount = new SqlCommand(query, con).ExecuteNonQuery();
							dt.Columns.Add(new DataColumn("Number of affected rows"));
							dt.Rows.Add(rowCount);
							break;
					}
				}
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
