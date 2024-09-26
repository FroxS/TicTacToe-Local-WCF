using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using TicTacToe.Domain;

namespace TicTacToe.Contract
{
    [ServiceContract(CallbackContract = typeof(ITicTacToeCallBack), SessionMode = SessionMode.Required)]
    public interface ITicTacToeService
    {
        [OperationContract(IsOneWay = false)]
        Task<char> ConnectAsync(User user);

        [OperationContract(IsOneWay = false)]
        ObservableCollection<User> GetConnectedUsers();

        [OperationContract(IsOneWay = false)]
        User GetOpponent(User user);

        [OperationContract(IsOneWay = false)]
        Task<bool> MoveAsync(ETicTacToePos pos, string player_id);

    }
}