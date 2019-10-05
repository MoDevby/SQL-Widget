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
		private const string FilePath = @".\FavoriteQueries.json";

		public static List<FavoriteItem> GetFavoriteList()
		{
			var res = new List<FavoriteItem>();
			if (File.Exists(FilePath))
				try { res = JsonConvert.DeserializeObject<List<FavoriteItem>>(File.ReadAllText(FilePath)); }
				catch (Exception ex) { MessageBox.Show(ex.Message, "Error reading the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }

			return res ?? new List<FavoriteItem>();
		}

		public static void AddNewFavoriteItem(FavoriteItem newItem)
		{
			var list = GetFavoriteList();
			newItem.Index = list.Any() ? list.Max(x => x.Index) + 1 : 0;
			list.Add(newItem);
			try { File.WriteAllText(FilePath, JsonConvert.SerializeObject(list)); }
			catch (Exception ex) { MessageBox.Show(ex.Message, "Error writing to the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		public static void UpdateFavoriteItem(FavoriteItem item)
		{
			var list = GetFavoriteList();
			var listItem = list.First(x => x.Index == item.Index);

			listItem.Description = item.Description;
			listItem.Query = item.Query;

			try { File.WriteAllText(FilePath, JsonConvert.SerializeObject(list)); }
			catch (Exception ex) { MessageBox.Show(ex.Message, "Error writing to the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		public static void RemoveFavoriteItem(FavoriteItem item)
		{
			var list = GetFavoriteList();
			list.Remove(item);

			try { File.WriteAllText(FilePath, JsonConvert.SerializeObject(list)); }
			catch (Exception ex) { MessageBox.Show(ex.Message, "Error writing to the favorite file", MessageBoxButton.OK, MessageBoxImage.Error); }
		}
	}
}
