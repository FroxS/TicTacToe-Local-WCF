using System;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.Text;
using TicTacToe.Service;
using TicTacToe.Contract;
using System.Threading.Tasks;
using System.Windows.Input;
using TicTacToe.Domain;

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

        private MainWindow _mainWindow;

        private bool _waitingForPlayer = false;
        private bool _end = false;
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

        public ICommand StartGameCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            RelayCommand.DefaultActionOnError = (ex) =>
            {
                SetMessage(ex.Message);
            };
            MoveCommand = new AsyncRelayCommand<ETicTacToePos>(Move, (o) => MyMove);
            StartGameCommand = new AsyncRelayCommand(() => StartSearchGame());
        }

        #endregion

        #region CallBack

        public void UserMoved(ETicTacToePos pos, char player)
        {
            SettoBoard(pos, player);
            MyMove = player != _me.Char;
            if(!MyMove)
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
            if(MyMove)
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

        private void SetMessage(string message)
        {
            Message =  message;
        }

        private void SetTitle(string title)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.Title = title;
            });
        }

        #endregion

        #region Client Server method

        public async Task StartSearchGame()
        {
            try
            {

                bool runned = true;
                string foundedAddress = FindAddress();
                if (string.IsNullOrEmpty(foundedAddress))
                {
                    WaitingForPlayer = true;
                    runned = RunServer();
                }
                if (runned)
                {
                    var user = new User()
                    {
                        TimeCreated = DateTime.Now,
                        Name = Environment.UserName
                    };

                    runned = await RunClientAsync(user);
                }
                if(runned)
                {
                    _mainWindow.SwitchView();
                }
                else
                {
                    System.Windows.MessageBox.Show("Nie udało się połączyć. spróbujemy ponownie");
                    StartSearchGame();
                }

            }catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public bool RunServer()
        {
            try
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
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
           
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Server Run");
        }

        public async Task<bool> RunClientAsync(User user)
        {
            _me = user;
            var uri = GetAddress(FindAddress());
            var callBack = new InstanceContext(this);
            var binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.FromSeconds(5), 
                ReceiveTimeout = TimeSpan.FromMinutes(5)
            };
            var chanel = new DuplexChannelFactory<ITicTacToeService>(callBack, binding);
            var endpoint = new EndpointAddress(uri);
            var proxy = chanel.CreateChannel(endpoint);
            if (proxy != null)
            {
                char c = await proxy.ConnectAsync(user);
                if(c != char.MinValue)
                {
                    _me.Char = c;
                    _proxy = proxy;
                    OnPropertyChanged(nameof(Player));
                    OnPropertyChanged(nameof(Name));
                    SetTitle($"[{c}] {user.Name}");
                    return true;
                }
            }
            return false;
        }

        private string FindAddress()
        {
            string serverIP = null;
            try
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Client.ReceiveTimeout = 5000;
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
            return $"net.tcp://{ip}:{SERVER_PORT}/TicTacToeService/";
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