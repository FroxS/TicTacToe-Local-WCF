using TicTacToe;
using TicTacToe.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using TicTacToe.Service;
using TicTacToe.Domain;
using System.Threading.Tasks;

namespace TicTacToe
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    internal class TicTacToeService : ITicTacToeService
    {
        #region Fields

        private ITicTacToeCallBack _callBack = null;
        private ObservableCollection<User> _users = null;
        private readonly Dictionary<string,ITicTacToeCallBack> _clients = null;
        private bool _canConnect = true;

        private TicTacToeGame _game;

        #endregion

        #region Public properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeService()
        {
            _users = new ObservableCollection<User>();
            _clients = new Dictionary<string, ITicTacToeCallBack>();
        }

        #endregion

        #region Metods

        public async Task<char> ConnectAsync(User user)
        {
            _callBack =OperationContext.Current.GetCallbackChannel<ITicTacToeCallBack>();
            if(_callBack != null)
            {
                user.Char = 'X';
                _clients.Add(user.UserId, _callBack);
                _users.Add(user);
                await Task.Run(() => {
                    _clients?.ToList().ForEach(c =>
                    {
                        if (c.Key != user.UserId)
                            c.Value.UserJoined(user);
                    });
                });
                
                Console.WriteLine($"{user.Name} just connected");
                if(_users.Count() == 2)
                {
                    user.Char = 'O';
                    _canConnect = false;
                    StartGame();
                }
                return user.Char;
            }
            return char.MinValue;
        }

        private void StartGame()
        {
            _game = new TicTacToeGame();
            char curent_player = _game.GetCurrentPlayer();
            _clients?.ToList().ForEach(c =>
            {
                c.Value.StartGame(GetOpponent(c.Key), GetUser(c.Key).Char, curent_player == GetUser(c.Key).Char);
            });
        }

        public async Task<bool> MoveAsync(ETicTacToePos pos, string player)
        {
            if (_game == null)
                return false;

            Console.WriteLine($"Player {_users.FirstOrDefault(u => u.UserId == player).Name} move pos: {pos}");

            var moved = _game.MakeMove((int)pos);
            if (!moved)
            {
                return false;
            }
            

            await Task.Run(() =>
            {
                _clients?.ToList().ForEach(c =>
                {
                    c.Value.UserMoved(pos, GetUser(player).Char);
                });
            });

            if (_game.CheckWin())
            {
                _clients?.ToList().ForEach(c =>
                {
                    User user = GetUser(c.Key);
                    if (_game.GetCurrentPlayer() == user.Char)
                        c.Value.Win();
                    else
                        c.Value.Lost();
                });
                return true;
            }
            _game.SwitchPlayer();
            if (_game.IsBoardFull())
            {
                _clients?.ToList().ForEach(c =>
                {
                    c.Value.Draw();
                });
                return true;
            }
            return true;
        }

        public ObservableCollection<User> GetConnectedUsers()
        {
            return _users;
        }

        public User GetOpponent(User user)
        {
            return GetOpponent(user.UserId);
        }

        public User GetOpponent(string id)
        {
            return _users.FirstOrDefault(x => x.UserId != id);
        }

        public User GetUser(string id)
        {
            return _users.FirstOrDefault(x => x.UserId == id);
        }

        //public void SendMessage(Message message)
        //{
        //    var sendto = _clients.First(c => c.Key == message.ToUserID);
        //    sendto.Value.ForwardToClient(message);
        //}

        #endregion
    }

}