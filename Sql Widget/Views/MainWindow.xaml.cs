using System.Windows;
using System.Windows.Controls;

namespace Sql_Widget.Views
{
    public partial class MainWindow : Window
    {
        //private const int CollapsedWidth = 68;
        private const int CollapsedHeight = 135;

        private const int ExpandedWidth = 465;
        private const int ExpandedHeight = 475;
        private const int ExpandedHeightWithServer = 617;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Expander expander)) return;

            if (expander.Name == mainExpander.Name)
            {
                this.SizeToContent = SizeToContent.Width;
                this.Height = CollapsedHeight;
                //RePosition(CollapsedWidth, CollapsedHeight);
                RePosition((int)this.Width, (int)this.Height);
            }
            else if (expander.Name == serverExpander.Name)
            {
                RePosition(ExpandedWidth, ExpandedHeight);
            }
        }

        private void MainExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Expander expander)) return;

            if (expander.Name == mainExpander.Name)
            {
                this.SizeToContent = SizeToContent.WidthAndHeight;
                RePosition(ExpandedWidth, (int)this.Height);
            }
            else if (expander.Name == serverExpander.Name)
                RePosition((int)this.Width, ExpandedHeightWithServer);
        }

        private void RePosition(int newWidth, int newHeight)
        {
            this.Left = SystemParameters.PrimaryScreenWidth - newWidth;
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (newHeight / 2);
        }

        //private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //	if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        //		this.DragMove();
        //}
    }
}
