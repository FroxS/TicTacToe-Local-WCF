using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TicTacToe.Contract;
using TicTacToe.Domain;
using TicTacToe.Service;

namespace TicTacToe.ViewModel
{
    public class TicTacToeGameViewModel : ObservableObject, ITicTacToeCallBack
    {
        #region Private properties

        private bool _myMove = false;
        private User _me;
        private User _opponent;
        private Board _board;

        private ITicTacToeService _proxy;

        private bool _waitingForPlayer = true;
        private bool _end = false;

        #endregion

        #region Public properties

        public User Opponent
        {
            get => _opponent;
            set => SetProperty(ref _opponent, value);
        }

        public Board Board
        {
            get => _board;
            private set => SetProperty(ref _board, value);
        }

        public bool MyMove
        {
            get => _myMove;
            set => SetProperty(ref _myMove, value);
        }

        public char Player
        {
            get => _me?.Char ?? char.MinValue;
        }

        public string Name
        {
            get => _me?.Name ?? "";
        }

        public bool WaitingForPlayer
        {
            get => _waitingForPlayer;
            set => SetProperty(ref _waitingForPlayer, value);
        }

        public bool End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        #endregion

        #region Command

        public ICommand MoveCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeGameViewModel()
        {
            MoveCommand = new AsyncRelayCommand<ETicTacToePos>(Move, (o) => MyMove);
        }

        #endregion

        #region CallBack

        public void UserMoved(ETicTacToePos pos, char player)
        {
            SettoBoard(pos, player);
            MyMove = player != _me.Char;
            if (!MyMove)
                SetMessage($"Oczekiwanie na ruch gracza");
            else
                SetMessage("Twoja kolej");
        }

        public void UserJoined(User user)
        {
            bool isGirl = false;
            string name = user.Name.ToLower();
            if (name[name.Length - 1] == 'a')
                isGirl = true;
            SetMessage($"Do gry dołączył{(isGirl ? "a" : "")} {user.Name}");
        }

        public void StartGame(User opponent, char player, bool start)
        {
            WaitingForPlayer = false;
            Opponent = opponent;
            SetMessage($"Gra wystartowała, twój przeciwnik to  {opponent.Name}");
            Board = new Board();
            MyMove = start;
            if (MyMove)
                SetMessage("Twoja kolej");
            else
                SetMessage($"Oczekiwanie na ruch gracza");
        }

        public void Win()
        {
            SetMessage("Gratulacje wygrałeś");
            MyMove = false;
        }

        public void Lost()
        {
            SetMessage("Nestety przegrałeś");
            MyMove = false;
        }

        public void Draw()
        {
            SetMessage("Gra zakończyła remisem");
            MyMove = false;
        }

        #endregion

        #region Command

        private async Task Move(ETicTacToePos pos)
        {
            if (MyMove)
            {
                if (Board[pos] == char.MinValue)
                {
                    var result = await _proxy.MoveAsync(pos, _me.UserId);
                    if (result)
                    {
                        SettoBoard(pos, _me.Char);
                        MyMove = false;
                    }
                }
            }
        }

        private void SettoBoard(ETicTacToePos pos, char player)
        {
            Board[pos] = player;
            OnPropertyChanged(nameof(Board));
        }

        #endregion

        #region Helper

        public void Setup(User user, ITicTacToeService proxy)
        {
            _me = user;
            _proxy = proxy;
            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(Name));
        }

        private void SetMessage(string message)
        {
            ObjectHost.GameService?.SetMessage(message);
        }

        #endregion

    }
}