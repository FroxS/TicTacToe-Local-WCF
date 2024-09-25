using TicTacToe;
using TicTacToe.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using TicTacToe.Service;
using TicTacToe.Domain;

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

        public void Connect(User user)
        {
            _callBack = OperationContext.Current.GetCallbackChannel<ITicTacToeCallBack>();
            if(_callBack != null)
            {
                user.Char = 'X';
                _clients.Add(user.UserId, _callBack);
                _users.Add(user);
                _clients?.ToList().ForEach(c => 
                {
                    if(c.Key != user.UserId)
                        c.Value.UserJoined(user);
                });
                Console.WriteLine($"{user.Name} just connected");
                if(_users.Count() == 2)
                {
                    user.Char = 'O';
                    _canConnect = false;
                    StartGame();
                }

            }
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

        public void Move(ETicTacToePos pos, string player)
        {
            if (_game == null)
                return;
            Console.WriteLine($"Player {_users.FirstOrDefault(u => u.UserId == player).Name} move pos: {pos}");
            _clients?.ToList().ForEach(c =>
            {
                if (c.Key != player)
                    c.Value.UserMoved(pos, GetUser(player).Char);

            });

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