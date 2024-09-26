using TicTacToe.Service;

namespace TicTacToe.Domain
{
    public class Board : ObservableObject
    {
        #region Fields

        private char _tl;
        private char _tm;
        private char _tr;
        private char _ml;
        private char _mm;
        private char _mr;
        private char _bl;
        private char _bm;
        private char _br;

        #endregion

        #region Properties

        public char TL
        {
            get => _tl;
            set => SetProperty(ref _tl, value);
        }

        public char TM
        {
            get => _tm;
            set => SetProperty(ref _tm, value);
        }

        public char TR
        {
            get => _tr;
            set => SetProperty(ref _tr, value);
        }

        public char ML
        {
            get => _ml;
            set => SetProperty(ref _ml, value);
        }

        public char MM
        {
            get => _mm;
            set => SetProperty(ref _mm, value);
        }

        public char MR
        {
            get => _mr;
            set => SetProperty(ref _mr, value);
        }

        public char BL
        {
            get => _bl;
            set => SetProperty(ref _bl, value);
        }

        public char BM
        {
            get => _bm;
            set => SetProperty(ref _bm, value);
        }

        public char BR
        {
            get => _br;
            set => SetProperty(ref _br, value);
        }

        public char this[ETicTacToePos pos]
        {
            get => Get((int)pos);
            set => Set((int)pos, value);
        }

        public char this[int pos]
        {
            get => Get(pos);
            set => Set(pos, value);
        } 

        #endregion

        #region Constructor

        public Board()
        {

        }

        #endregion

        private char Get(int pos)
        {
            switch (pos)
            {
                case 1:
                    return TL;
                case 2:
                    return TM;
                case 3:
                    return TR;
                case 4:
                    return ML;
                case 5:
                    return MM;
                case 6:
                    return MR;
                case 7:
                    return BL;
                case 8:
                    return BM;
                case 9:
                    return BR;
                default:
                    return char.MinValue;
            }
        }

        private void Set(int pos, char val)
        {
            switch (pos)
            {
                case 1:
                    TL = val;
                    break;
                case 2:
                    TM = val;
                    break;
                case 3:
                    TR = val;
                    break;
                case 4:
                    ML = val;
                    break;
                case 5:
                    MM = val;
                    break;
                case 6:
                    MR = val;
                    break;
                case 7:
                    BL = val;
                    break;
                case 8:
                    BM = val;
                    break;
                case 9:
                    BR = val;
                    break;
            }
        }

        public char CheckWin()
        {
            // Sprawdzenie wierszy
            if (TL == TM && TM == TR && TL != char.MinValue) return TL;
            if (ML == MM && MM == MR && ML != char.MinValue) return ML;
            if (BL == BM && BM == BR && BL != char.MinValue) return BL;

            // Sprawdzenie kolumn
            if (TL == ML && ML == BL && TL != char.MinValue) return TL;
            if (TM == MM && MM == BM && TM != char.MinValue) return TM;
            if (TR == MR && MR == BR && TR != char.MinValue) return TR; 

            // Sprawdzenie przekątnych
            if (TL == MM && MM == BR && TL != char.MinValue) return TL; 
            if (TR == MM && MM == BL && TR != char.MinValue) return TR; 

            return char.MinValue;
        }

        public bool IsFull()
        {
            for (ETicTacToePos pos = ETicTacToePos.TL; pos <= ETicTacToePos.BR; pos++)
            {
                if (Get((int)pos) == char.MinValue)
                    return false;
            }
            return true;
        }

        public (ETicTacToePos?, ETicTacToePos?, ETicTacToePos?) GetWinRows()
        {
            // Sprawdzenie wierszy
            if (TL == TM && TM == TR && TL != char.MinValue) return (ETicTacToePos.TL, ETicTacToePos.TM, ETicTacToePos.TR); // Wiersz górny
            if (ML == MM && MM == MR && ML != char.MinValue) return (ETicTacToePos.ML, ETicTacToePos.MM, ETicTacToePos.MR); // Wiersz środkowy
            if (BL == BM && BM == BR && BL != char.MinValue) return (ETicTacToePos.BL, ETicTacToePos.BM, ETicTacToePos.BR); // Wiersz dolny

            // Sprawdzenie kolumn
            if (TL == ML && ML == BL && TL != char.MinValue) return (ETicTacToePos.TL, ETicTacToePos.ML, ETicTacToePos.BL); // Kolumna lewa
            if (TM == MM && MM == BM && TM != char.MinValue) return (ETicTacToePos.TM, ETicTacToePos.MM, ETicTacToePos.BM); // Kolumna środkowa
            if (TR == MR && MR == BR && TR != char.MinValue) return (ETicTacToePos.TR, ETicTacToePos.MR, ETicTacToePos.BR); // Kolumna prawa

            // Sprawdzenie przekątnych
            if (TL == MM && MM == BR && TL != char.MinValue) return (ETicTacToePos.TL, ETicTacToePos.MM, ETicTacToePos.BR); // Przekątna od lewego górnego do prawego dolnego
            if (TR == MM && MM == BL && TR != char.MinValue) return (ETicTacToePos.TR, ETicTacToePos.MM, ETicTacToePos.BL); // Przekątna od prawego górnego do lewego dolnego

            // Jeśli nie ma zwycięzcy, zwracamy null (brak wygranej)
            return (null, null, null);
        }

    }
}