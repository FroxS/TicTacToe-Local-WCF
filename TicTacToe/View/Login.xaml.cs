using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        #region Fields

        private MainWindow _window;

        #endregion

        public Login()
        {
            InitializeComponent();
            _window = (MainWindow) Application.Current.MainWindow;
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = _window.ViewModel;
            Task.Run(() => 
            {
                vm.StartSearchGame();
            });
        }
    }
}
