namespace Sql_Widget.Classes
{
	class HistoryItem
	{
		public string Date { get; set; }
		public string DBName { get; set; }
		public string Query { get; set; }
		public int RowCount { get; set; }
		public bool Succeeded { get; set; }
	}
}
