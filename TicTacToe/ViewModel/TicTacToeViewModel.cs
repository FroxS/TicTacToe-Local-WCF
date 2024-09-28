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
using System.Windows.Threading;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace TicTacToe.ViewModel
{
    internal class TicTacToeViewModel : ObservableObject, IGameServieVM
    {
        #region Private properties

        private const string REQUEST_NAME_SEARCH_SERVER = "DISCOVER_TICTACTOE_SERVER";
        private const string SERVER_ADDRES_REQUEST_NAME_KEY = "TICTACTOE_SERVER";
        private const string SERVER_LOBBY_KEY = "TICTACTOE_LOBBY";
        private const int SERVER_PORT = 6565;
        private const int SEARCH_SERVER_PORT = 9876;

        private string _message;
        private string _lobbyKey;
        private bool _isTaskRunning;
        private bool _canRunServer = true;
        private bool _canJoin = true;

        private string _selectedServer;
        private ObservableCollection<string> _aviableServers = new ObservableCollection<string>();

        #endregion

        #region Public properties

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message,value);
        }

        public string LobbyKey
        {
            get => _lobbyKey;
            set => SetProperty(ref _lobbyKey, value);
        }

        public string SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        public bool IsTaskRunning
        {
            get => _isTaskRunning;
            set => SetProperty(ref _isTaskRunning, value);
        }

        public bool CanJoin
        {
            get => _canJoin;
            set => SetProperty(ref _canJoin, value);
        }

        public bool CanRunServer
        {
            get => _canRunServer;
            set => SetProperty(ref _canRunServer, value);
        }

        public ObservableCollection<string> AviableServers
        {
            get => _aviableServers;
            set => SetProperty(ref _aviableServers, value);
        }

        public TicTacToeGameViewModel Game { get; private set; }

        #endregion

        #region Command

        public ICommand StartGameCommand { get; private set; }

        public ICommand RunServerCommand { get; private set; }

        public ICommand JoinCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeViewModel()
        {
            StartGameCommand = new AsyncRelayCommand(() => StartSearchGame());
            RunServerCommand = new AsyncRelayCommand(async () => 
            {
                IsTaskRunning = true;
                SetMessage("Uruchamiam Server");
                bool result =RunServer();
                if (result)
                {
                    result = await RunClientAsync(new User());
                    if (result)
                    {
                        ObjectHost.MainVindow.SwitchView();
                    }
                        
                }
                IsTaskRunning = false;
                //ClearMessage();

            });
            JoinCommand = new AsyncRelayCommand(Join);
        }

        private async Task Join()
        {
            if (!string.IsNullOrEmpty(SelectedServer))
            {
                CanRunServer = false;
                IsTaskRunning = true;
                SetMessage($"Dołączam do servera {LobbyKey}");
                LobbyKey = SelectedServer;
                var result = await RunClientAsync(new User());
                IsTaskRunning = false;
                ClearMessage();
                if (result)
                    ObjectHost.MainVindow.SwitchView();

                CanRunServer = true;
            }
        }

        #endregion

        #region Helper

        public void FillServers()
        {
            IsTaskRunning = true;
            SetMessage($"Szukam dostępnych serverów");
            var results = GetInfoServer();
            var disp = ObjectHost.Get<Dispatcher>();
            disp.Invoke(() => {

                var selected = SelectedServer;

                var actual = AviableServers.ToArray();
                var tocheck = results.Select(x => x.Count() > 1 ? x[1] : "").Where(x => !string.IsNullOrEmpty(x)).ToArray();

                if (!tocheck.SequenceEqual(actual))
                {
                    AviableServers.Clear();
                    foreach (var result in results)
                    {
                        if (result.Count() > 1)
                        {
                            AviableServers.Add(result[1]);
                        }
                    }
                }
                
                SelectedServer = selected;
                if (string.IsNullOrEmpty(SelectedServer))
                    SelectedServer = AviableServers.FirstOrDefault();

            });
            IsTaskRunning = false;
            ClearMessage();
        }

        public void SetMessage(string message)
        {
            Message =  message;
        }

        public void ClearMessage()
        {
            Message = "";
        }

        private void SetTitle(string title)
        {
            ObjectHost.Get<Dispatcher>()?.Invoke(() =>
            {
                ObjectHost.MainVindow.Title = title;
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
                    runned = RunServer();
                }
                if (runned)
                {
                    var user = new User();

                    runned = await RunClientAsync(user);
                }
                if(runned)
                {
                    ObjectHost.MainVindow.SwitchView();
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
                CanRunServer = false;
                IsTaskRunning = true;
                SetMessage("Tworzę serwer");
                var uris = new Uri[1];
                string lobby = RandomString();
                LobbyKey = lobby;
                string adr = GetAddress();
                uris[0] = new Uri(adr);
                ITicTacToeService service = new TicTacToeService();
                ServiceHost host = new ServiceHost(service, uris);
                var binding = new NetTcpBinding(SecurityMode.None);
                host.AddServiceEndpoint(typeof(ITicTacToeService), binding, /*String.Empty*/ adr);
                host.Opened += Host_Opened;
                host.Open();
                SetTitle($"Server - {LobbyKey}");
                Task.Run(() => { StartListeningServer(); });
                IsTaskRunning = false;
                ClearMessage();
                return true;
            }
            catch(AddressAlreadyInUseException ex)
            {
                SetMessage("Server już jest odpalony na tym komputerze");
                return false;
            }
            catch(Exception ex)
            {
                
                ClearMessage();
                return false;
            }
            finally
            {
                IsTaskRunning = false;
                CanRunServer = true;
            }
           
        }

        public async Task<bool> RunClientAsync(User user)
        {
            Game = new TicTacToeGameViewModel();
            OnPropertyChanged(nameof(Game));
            var uri = GetAddress(FindAddress());
            var callBack = new InstanceContext(Game);
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
                    
                    user.Char = c;
                    Game.Setup(user, proxy);
                    SetTitle($"[{c}] {user.Name} - {LobbyKey}");
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
                var result = GetInfoServer();
                if (result != null && result.Count > 0)
                    serverIP = result[0][0];

            }
            catch(Exception ex)
            {

            }
            return serverIP;
        }

        private List<string[]> GetInfoServer()
        {
            List<string[]> infoserver = new List<string[]>();
            try
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Client.ReceiveTimeout = 5000;
                udpClient.EnableBroadcast = true;
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, SEARCH_SERVER_PORT);
                byte[] request = Encoding.UTF8.GetBytes(REQUEST_NAME_SEARCH_SERVER);
                udpClient.Send(request, request.Length, ep);
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, SEARCH_SERVER_PORT);
                while(true)
                {
                    byte[] serverResponse = udpClient.Receive(ref serverEP);
                    string serverInfo = Encoding.UTF8.GetString(serverResponse);
                    if (serverInfo.StartsWith(SERVER_ADDRES_REQUEST_NAME_KEY))
                    {
                        var infos = serverInfo.Split(';');
                        var data = new string[2];
                        for (int i = 0; i < infos.Length; i++)
                        {
                            data[i] = infos[i].Split(':')[1];

                        }
                        infoserver.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return infoserver;
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
                    string message = $"{SERVER_ADDRES_REQUEST_NAME_KEY}:" + GetLocalIPAddress();
                    message += $";{SERVER_LOBBY_KEY}:{LobbyKey}";
                    byte[] response = Encoding.UTF8.GetBytes(message);
                    udpServer.Send(response, response.Length, remoteEP);
                }
            }
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Server Run");
        }

        #endregion

        #region Address methods

        private string GetAddress(string ip = null)
        {
            if (string.IsNullOrEmpty(ip))
                ip = GetLocalIPAddress();
            return $"net.tcp://{ip}:{SERVER_PORT}/TicTacToeService/{LobbyKey}";
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

        #endregion

        #region Helpers

        public string RandomString(int length = 4)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        #endregion
    }
}