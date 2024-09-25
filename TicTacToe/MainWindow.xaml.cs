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
           
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new TicTacToeViewModel();
        }

        #endregion
    }
}
