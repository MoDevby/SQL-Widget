using System;
using System.Data;
using System.Data.SqlClient;

namespace Sql_Widget.Models
{
	class QueryModel
	{
		public static string GetConnectionString(string dbName)
		{
			var server = Properties.Settings.Default.Server;
			var credentials = string.IsNullOrWhiteSpace(Properties.Settings.Default.UserName)
				? "Integrated Security=true"
				: $"User Id={Properties.Settings.Default.UserName}; Password={Properties.Settings.Default.Password}";

			return $"server={server};Database={dbName};{credentials}";
		}


		public DataTable Execute(string dbName, string query)
		{
			using (SqlConnection con = new SqlConnection(GetConnectionString(dbName)))
			{
				//Thread.Sleep(1000);
				DataTable dt = new DataTable();
				try
				{
					query = query.Trim();
					var firstWord = query.IndexOf(" ") > -1
						? query.Substring(0, query.TrimStart().IndexOf(" "))
						: query;
					switch (firstWord.ToLower())
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
