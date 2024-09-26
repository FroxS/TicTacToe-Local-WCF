using System.Collections.ObjectModel;
using System.ServiceModel;
using TicTacToe.Domain;

namespace TicTacToe.Contract
{
    public interface ITicTacToeCallBack
    {
        [OperationContract(IsOneWay = true)]
        void UserMoved(ETicTacToePos pos, char player);

        [OperationContract(IsOneWay = true)]
        void UserJoined(User user);

        [OperationContract(IsOneWay = true)]
        void StartGame(User opponent, char player, bool start);

        [OperationContract(IsOneWay = true)]
        void Win();

        [OperationContract(IsOneWay = true)]
        void Lost();

        [OperationContract(IsOneWay = true)]
        void Draw();
    }
}