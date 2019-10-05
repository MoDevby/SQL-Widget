using Newtonsoft.Json;

namespace Sql_Widget.Entities
{
	public enum VariablesNames
	{
		VAR_1 = 1,
		VAR_2,
		VAR_3
	}
	class FavoriteItem
	{
		[JsonIgnore]
		public bool Selected { get; set; }
		[JsonIgnore]
		public string QueryWithVariables
		{
			get
			{
				return Query.Replace('@' + VariablesNames.VAR_1.ToString(), Var1)
					.Replace('@' + VariablesNames.VAR_2.ToString(), Var2)
					.Replace('@' + VariablesNames.VAR_3.ToString(), Var3);
			}
		}

		public int Index { get; set; }
		public string Query { get; set; }
		public string Description { get; set; }
		public string Var1 { get; set; }
		public string Var2 { get; set; }
		public string Var3 { get; set; }
	}
}
