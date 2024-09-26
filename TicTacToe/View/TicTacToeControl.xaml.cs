using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TicTacToe.Domain;
using TicTacToe.Service;

namespace TicTacToe.View
{
    /// <summary>
    /// Interaction logic for TicTacToeControl.xaml
    /// </summary>
    public partial class TicTacToeControl : UserControl
    {
        #region Fields

        private Dictionary<ETicTacToePos, Button> _buttons = new Dictionary<ETicTacToePos, Button>();

        private bool isInited = false;

        private WaitControl _waitControl;

        private Label _textCtrl;
        #endregion

        #region Properties

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        public RelayCommand<ETicTacToePos> ClickCommand
        {
            get { return (RelayCommand<ETicTacToePos>)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public char Player
        {
            get { return (char)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        public bool WaitingForOpponents
        {
            get { return (bool)GetValue(WaitingForOpponentsProperty); }
            set { SetValue(WaitingForOpponentsProperty, value); }
        }

        #endregion

        #region Dependency

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board), typeof(Board), typeof(TicTacToeControl), new PropertyMetadata(null, BoardChanged, BoardChanging));

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register(nameof(ClickCommand), typeof(RelayCommand<ETicTacToePos>), typeof(TicTacToeControl), new PropertyMetadata(null, ClickCommandChanged));

        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register(nameof(Player), typeof(char), typeof(TicTacToeControl), new PropertyMetadata(char.MinValue));

        public static readonly DependencyProperty WaitingForOpponentsProperty =
            DependencyProperty.Register(nameof(WaitingForOpponents), typeof(bool), typeof(TicTacToeControl), new PropertyMetadata(true, WaitingForOpponentsChanged, WaitingForOpponentsChanging));

        #endregion

        #region Constructor

        public TicTacToeControl()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Dependecy method

        private static void BoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TicTacToeControl tc && e.NewValue is Board b && b != null)
            {
                tc.Update();
            }
        }

        private static object BoardChanging(DependencyObject d, object baseValue)
        {
            if (d is TicTacToeControl tc)
            {
                tc.Update();
            }
            return baseValue;
        }

        private static void ClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TicTacToeControl tc)
            {
                foreach (var button in tc._buttons)
                {
                    button.Value.Command = tc.ClickCommand ;
                }
            }
        }

        private static void WaitingForOpponentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TicTacToeControl tc)
            {

                tc.UpdateWaitingControll();
            }
        }

        private static object WaitingForOpponentsChanging(DependencyObject d, object baseValue)
        {
            if (d is TicTacToeControl tc)
            {
                tc.UpdateWaitingControll();

            }

            return baseValue;
        }

        private void UpdateWaitingControll()
        {
            Grid grid = (Content as Grid);
            if (grid != null)
                grid.IsEnabled = !WaitingForOpponents;
            if (WaitingForOpponents)
            {
                _textCtrl.Visibility = _waitControl.Visibility = Visibility.Visible;
            }
            else
            {

                _textCtrl.Visibility = _waitControl.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Methods

        private void Update()
        {
            if(Board != null) 
            { 
                foreach(var button in _buttons)
                {
                    button.Value.Content = Board[button.Key];
                    button.Value.Command = ClickCommand;
                    
                }
                UpdateWin();
            }
        }

        private void Init()
        {
            _buttons.Clear();
            var grid = new Grid();
            
            for (int i = 0; i < 3; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
            }
            ETicTacToePos pos = ETicTacToePos.TL ;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var button = new Button() { CommandParameter = pos };
                    grid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    _buttons.Add(pos, button);
                    pos++;
                }
            }

            _waitControl = new WaitControl();
            grid.Children.Add(_waitControl);
            Grid.SetColumnSpan(_waitControl, 3);
            Grid.SetRowSpan(_waitControl, 3);

            _textCtrl = new Label() {
                Content = "Oczekiwanie na gracza",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 5),
            };
            grid.Children.Add(_textCtrl);
            Grid.SetColumnSpan(_textCtrl, 3);
            Grid.SetRowSpan(_textCtrl, 3);

            Content = grid;
            isInited = true;
        }

        private void UpdateWin()
        {
            if(Board != null && Player != char.MinValue)
            {
                var player = Player;
                var win = Board.CheckWin();
                if(win != char.MinValue)
                {
                    var winrow = Board.GetWinRows();
                    SolidColorBrush color = win == player ? Brushes.Green : Brushes.Red;
                    _buttons[winrow.Item1.Value].Background = color;
                    _buttons[winrow.Item2.Value].Background = color;
                    _buttons[winrow.Item3.Value].Background = color;
                }

                if (Board.IsFull())
                {
                    foreach (var btn in _buttons)
                        btn.Value.Background = Brushes.Orange;
                }

            }
        }

        #endregion
    }
}
