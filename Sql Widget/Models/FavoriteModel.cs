using Newtonsoft.Json;
using Sql_Widget.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Sql_Widget.Models
{
	static class FavoriteModel
	{
		private const string FilePath = @".\Favorite.json";

		public static List<FavoriteItem> GetFavoriteList()
		{
			var res = new FavoriteWraper();
			if (File.Exists(FilePath))
				try { res = JsonConvert.DeserializeObject<FavoriteWraper>(File.ReadAllText(FilePath)) ?? res; }
				catch (Exception ex) { MessageBox.Show(ex.Message, "Error reading the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }

			return res.FavoriteList;
		}

		public static List<FavoriteItem> AddNewFavoriteItem(FavoriteItem newItem)
		{
			var list = GetFavoriteList();
			newItem.Index = list.Any() ? list.Max(x => x.Index) + 1 : 0;
			list.Add(newItem);

			WriteToFile(list);
			return list;
		}

		public static List<FavoriteItem> UpdateFavoriteItem(FavoriteItem item)
		{
			var list = GetFavoriteList();
			var listItem = list.First(x => x.Index == item.Index);

			listItem.Description = item.Description;
			listItem.Query = item.Query;
			listItem.Var1 = item.Var1;
			listItem.Var2 = item.Var2;
			listItem.Var3 = item.Var3;

			WriteToFile(list);
			return list;
		}

		public static List<FavoriteItem> RemoveFavoriteItem(FavoriteItem item)
		{
			var list = GetFavoriteList();
			list.Remove(list.FirstOrDefault(a => a.Index == item.Index));

			WriteToFile(list);
			return list;
		}

		private static void WriteToFile(List<FavoriteItem> list)
		{
			try { File.WriteAllText(FilePath, JsonConvert.SerializeObject(new FavoriteWraper { FavoriteList = list })); }
			catch (Exception ex) { MessageBox.Show(ex.Message, "Error writing to the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private class FavoriteWraper
		{
			public List<FavoriteItem> FavoriteList { get; set; } = new List<FavoriteItem>();
		}
	}
}
