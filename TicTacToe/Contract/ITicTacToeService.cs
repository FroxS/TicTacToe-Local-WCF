using System.Collections.ObjectModel;
using System.ServiceModel;
using TicTacToe.Domain;

namespace TicTacToe.Contract
{
    [ServiceContract(CallbackContract = typeof(ITicTacToeCallBack), SessionMode = SessionMode.Required)]
    public interface ITicTacToeService
    {
        [OperationContract(IsOneWay = true)]
        void Connect(User user);

        //[OperationContract(IsOneWay = true)]
        //void SendMessage(Message message);

        [OperationContract(IsOneWay = false)]
        ObservableCollection<User> GetConnectedUsers();

        [OperationContract(IsOneWay = false)]
        User GetOpponent(User user);

        [OperationContract(IsOneWay = true)]
        void Move(ETicTacToePos pos, string player_id);
    }
}