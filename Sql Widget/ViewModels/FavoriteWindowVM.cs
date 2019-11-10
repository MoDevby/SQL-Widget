using Sql_Widget.Entities;
using Sql_Widget.Helper;
using Sql_Widget.Models;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sql_Widget.ViewModels
{
	class FavoriteWindowVM : INotifyPropertyChanged
	{
#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		#region Properites
		public Action CloseAction { get; set; }

		public int? Index { get; set; } = null;
		public string Description { get; set; }
		public string Query { get; set; }

		public string ActionButtonTitle => Index == null ? "Add" : "Update";
		public bool IsValidFavoriteItem =>
			!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Query);

		private FavoriteItem item => new FavoriteItem
		{
			Index = Index ?? 0,
			Description = Description,
			Query = Query
		};
		#endregion

		#region Methods
		public FavoriteWindowVM(FavoriteItem existingItem = null)
		{
			if (existingItem == null) return;

			Index = existingItem.Index;
			Description = existingItem.Description;
			Query = existingItem.Query;
		}

		#endregion

		#region Commands
		public ICommand ActionCommand => new ButtonsCommand((object obj) =>
		{
			if (!IsValidFavoriteItem) return;

			var res = (Index == null) ? FavoriteModel.AddNewFavoriteItem(item) : FavoriteModel.UpdateFavoriteItem(item);
			CloseAction();
		});

		public ICommand CloseCommand => new ButtonsCommand((object obj) => CloseAction());

		#endregion
	}
}
