using Sql_Widget.Helper;

namespace Sql_Widget.Classes
{
	class ConditionElement
	{
		public ConditionElement()
		{
			SelectedField = string.Empty;
			SelectedOerator = 0;
			Value = string.Empty;
		}
		public string SelectedField { get; set; }
		public CompareOperators SelectedOerator { get; set; }
		public string Value { get; set; }
		public RelationOperators NextLineOperator{ get; set; }
	}
}
