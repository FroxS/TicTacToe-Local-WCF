using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TicTacToe.Domain;
using TicTacToe.Service;
using TicTacToe.ViewModel;

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
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            IMainWindow vm = ObjectHost.MainVindow;
            if(vm?.ViewModel != null) 
            {
                Task.Run(() =>
                {
                    vm.ViewModel.StartSearchGame();
                });
            }
            
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            TicTacToeViewModel vm =DataContext as TicTacToeViewModel;
            if (vm != null)
            {
                Task.Run(() => { vm.FillServers(); });
            }
                
        }
    }
}
