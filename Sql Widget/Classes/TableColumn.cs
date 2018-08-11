namespace Sql_Widget.Classes
{
	public class TableColumn
	{
		public int Position { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public int? MaxLength { get; set; }
		public byte? Precision { get; set; }
		public bool IsNullable { get; set; }
	}
}
