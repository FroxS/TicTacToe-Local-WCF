using System;
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

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserName.Text))
            {
                User user  = new User() {
                    TimeCreated = DateTime.Now,
                    Name = UserName.Text
                };
                _window.ViewModel.RunClient(user);

                _window.Main.Children.Clear();
                _window.Main.Children.Add(new TicTacToeBoard());
            }
        }


        private void RunServerBtn_Click(object sender, RoutedEventArgs e)
        {
            _window.ViewModel.RunServer();
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Tcp Service Started");
        }
    }
}
