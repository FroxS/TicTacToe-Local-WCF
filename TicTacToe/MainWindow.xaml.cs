using TicTacToe.ViewModel;
using System.Windows;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        internal TicTacToeViewModel ViewModel 
        {
            get => DataContext as TicTacToeViewModel;
            set => DataContext = value; 
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new TicTacToeViewModel(this);
        }

        #endregion

        #region Method

        internal void SwitchView()
        {
            Dispatcher.Invoke(() =>
            {

                if (Main.Children[0].Visibility == Visibility.Visible)
                {
                    Main.Children[0].Visibility = Visibility.Collapsed;
                    Main.Children[1].Visibility = Visibility.Visible;
                }
                else
                {
                    Main.Children[1].Visibility = Visibility.Collapsed;
                    Main.Children[0].Visibility = Visibility.Visible;
                }
            });
            
        }

        #endregion
    }
}
