using Sql_Widget.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sql_Widget.Models
{
	class OperatorsModel
	{
		public static Dictionary<CompareOperators, string> GetCompareOperators()
		{
			return new Dictionary<CompareOperators, string>
			{
				{ CompareOperators.Equal , "=" },
				{ CompareOperators.NotEqual, "<>" },
				{ CompareOperators.Greater , ">" },
				{ CompareOperators.Less , "<" },
				{ CompareOperators.GreaterOrEqual , ">=" },
				{ CompareOperators.LessOrEqual , "<=" },
				{ CompareOperators.Like , "LIKE" },
			};
		}

		public static List<RelationOperators> GetReleationsOperators()
		{
			return Enum.GetValues(typeof(RelationOperators)).Cast<RelationOperators>().ToList();
		}
	}
}
