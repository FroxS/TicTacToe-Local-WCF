using TicTacToe.ViewModel;
using System.Windows;
using TicTacToe.Domain;
using TicTacToe.Service;
using System.Windows.Threading;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        #region Properties

        public IGameServieVM ViewModel 
        {
            get => GetVM();
            private set => DataContext = value; 
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new TicTacToeViewModel();
            ObjectHost.Setup(this);
            ObjectHost.Set(Dispatcher);
            RelayCommand.DefaultActionOnError = (ex) =>
            {
                ViewModel?.SetMessage(ex.Message);
            };
            
        }

        #endregion

        #region Method

        private IGameServieVM GetVM()
        {
            IGameServieVM vm = null;
            Dispatcher.Invoke(() => { vm = DataContext as IGameServieVM; });
            return vm;
        }

        public void SwitchView()
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
