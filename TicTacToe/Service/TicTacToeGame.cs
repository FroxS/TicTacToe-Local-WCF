namespace TicTacToe.Service
{
    public class TicTacToeGame
    {
        #region Private properties

        private char[,] board;

        private char currentPlayer;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicTacToeGame(char start_player = 'X')
        {
            board = new char[3, 3]
            {
            { '1', '2', '3' },
            { '4', '5', '6' },
            { '7', '8', '9' }
            };
            currentPlayer = start_player; // Gracz X zaczyna
        }

        #endregion

        #region Methods

        public bool MakeMove(int move)
        {
            int row = (move - 1) / 3;
            int col = (move - 1) % 3;

            if (board[row, col] != 'X' && board[row, col] != 'O')
            {
                board[row, col] = currentPlayer;
                return true;
            }
            return false;
        }

        public bool CheckWin()
        {
            // Sprawdzenie wierszy
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == currentPlayer && board[i, 1] == currentPlayer && board[i, 2] == currentPlayer)
                {
                    return true;
                }
            }

            // Sprawdzenie kolumn
            for (int i = 0; i < 3; i++)
            {
                if (board[0, i] == currentPlayer && board[1, i] == currentPlayer && board[2, i] == currentPlayer)
                {
                    return true;
                }
            }

            // Sprawdzenie przekątnych
            if ((board[0, 0] == currentPlayer && board[1, 1] == currentPlayer && board[2, 2] == currentPlayer) ||
                (board[0, 2] == currentPlayer && board[1, 1] == currentPlayer && board[2, 0] == currentPlayer))
            {
                return true;
            }

            return false;
        }

        public bool IsBoardFull()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != 'X' && board[i, j] != 'O')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public char GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
        }

        #endregion
    }
}