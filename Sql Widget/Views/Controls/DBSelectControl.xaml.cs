using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sql_Widget.Views.Controls
{
	public partial class DBSelectControl : UserControl
	{
		public DBSelectControl()
		{
			InitializeComponent();
			this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(DBandTableControl_IsVisibleChanged);

		}

		private void DBandTableControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue == true)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate () { DBSelector.Focus(); }));
			}
		}
	}
}
