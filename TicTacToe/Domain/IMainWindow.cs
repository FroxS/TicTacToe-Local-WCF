using System.Threading.Tasks;
using System.Windows.Threading;

namespace TicTacToe.Domain
{
    public interface IMainWindow
    {
        IGameServieVM ViewModel { get; }
        string Title { get; set; }
        void SwitchView();

    }

    public interface IGameServieVM
    {
        void SetMessage(string message);
        Task StartSearchGame();
    }
}