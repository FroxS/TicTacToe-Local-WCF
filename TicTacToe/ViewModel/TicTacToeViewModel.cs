using System;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.Text;
using TicTacToe.Service;
using TicTacToe.Contract;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TicTacToe.Domain;
using System.Windows.Documents;

namespace TicTacToe.ViewModel
{
    internal class TicTacToeViewModel : ObservableObject, ITicTacToeCallBack
    {
        #region Private properties

        private const string REQUEST_NAME_SEARCH_SERVER = "DISCOVER_TICTACTOE_SERVER";
        private const string SERVER_ADDRES_REQUEST_NAME_KEY = "TICTACTOE_SERVER";
        private const int SERVER_PORT = 6565;
        private const int SEARCH_SERVER_PORT = 9876;

        private string _message;
        private bool _myMove = false;
        private User _me;
        private User _opponent;
        private Board _board;

        private ITicTacToeService _proxy;

        #endregion

        #region Public properties

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message,value);
        }

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


        #endregion

        #region Command

        public ICommand MoveCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeViewModel()
        {
            RelayCommand.DefaultActionOnError = (ex) =>
            {
                SetMessage(ex.Message);
            };

            MoveCommand = new RelayCommand<ETicTacToePos>(Move, (o) => MyMove);
        }

        #endregion

        #region CallBack

        public void UserMoved(ETicTacToePos pos, char player)
        {
            int move = (int)pos;
            int row = (move - 1) / 3;
            int col = (move - 1) % 3;
            Board[pos] = player.ToString();
            OnPropertyChanged(nameof(Board));
            SetMessage($"player {player} add pos:{pos}");
            MyMove = _me.Char != player;
        }

        public void UserJoined(User user)
        {
            SetMessage($"player {user.Name} joined");
        }

        public void StartGame(User opponent, char player, bool start)
        {
            Opponent = opponent;
            SetMessage($"Start game, your opponent is  {opponent.Name}");
            Board = new Board();
            MyMove = start;
        }

        #endregion

        #region Command

        private void Move(ETicTacToePos pos)
        {
            if (MyMove)
            {
                if (Board[pos] == "")
                {
                    _proxy.Move(pos, _me.UserId);
                    Board[pos] = _me.Char.ToString();
                }
                    
            }
            
        }

        #endregion

        #region Helper

        private void SetMessage(string message)
        {
            Message += "\n\r " + message;
        }

        #endregion

        #region Client Server method

        public void RunServer()
        {
            var uris = new Uri[1];
            string adr = GetAddress();
            uris[0] = new Uri(adr);
            ITicTacToeService service = new TicTacToeService();
            ServiceHost host = new ServiceHost(service, uris);
            var binding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(ITicTacToeService), binding, String.Empty);
            host.Opened += Host_Opened;
            host.Open();
            Task.Run(() => { StartListeningServer(); });
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Server Run");
        }

        public void RunClient(User user)
        {
            _me = user;
            var uri = GetAddress(FindAddress());
            var callBack = new InstanceContext(this/*new TicTacToeCallBack()*/);
            var binding = new NetTcpBinding(SecurityMode.None);
            var chanel = new DuplexChannelFactory<ITicTacToeService>(callBack, binding);
            var endpoint = new EndpointAddress(uri);
            var proxy = chanel.CreateChannel(endpoint);
            if (proxy != null)
            {
                _proxy = proxy;
                proxy.Connect(user);
            }
        }

        private string FindAddress()
        {
            string serverIP = null;
            try
            {
                UdpClient udpClient = new UdpClient();
                udpClient.EnableBroadcast = true;
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, SEARCH_SERVER_PORT);
                byte[] request = Encoding.UTF8.GetBytes(REQUEST_NAME_SEARCH_SERVER);
                udpClient.Send(request, request.Length, ep);
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, SEARCH_SERVER_PORT);
                byte[] serverResponse = udpClient.Receive(ref serverEP);
                string serverInfo = Encoding.UTF8.GetString(serverResponse);
                if (serverInfo.StartsWith(SERVER_ADDRES_REQUEST_NAME_KEY))
                {
                    serverIP = serverInfo.Split(':')[1];
                }
                
            }
            catch(Exception ex)
            {

            }
            return serverIP;
        }

        private string GetAddress(string ip = null)
        {
            if (string.IsNullOrEmpty(ip))
                ip = GetLocalIPAddress();
            return $"net.tcp://{ip}:{SERVER_PORT}/TicTacToeService/"; //"net.tcp://localhost:6565/MessageService";
        }

        private string GetLocalIPAddress()
        {
            string IP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = ip.ToString();
                }
            }
            return IP;
        }

        private void StartListeningServer()
        {
            UdpClient udpServer = new UdpClient(SEARCH_SERVER_PORT);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, SEARCH_SERVER_PORT);
            while (true)
            {
                byte[] data = udpServer.Receive(ref remoteEP);
                string request = Encoding.UTF8.GetString(data);

                if (request == REQUEST_NAME_SEARCH_SERVER)
                {
                    byte[] response = Encoding.UTF8.GetBytes($"{SERVER_ADDRES_REQUEST_NAME_KEY}:" + GetLocalIPAddress());
                    udpServer.Send(response, response.Length, remoteEP);
                }
            }
        }

        #endregion

    }
}