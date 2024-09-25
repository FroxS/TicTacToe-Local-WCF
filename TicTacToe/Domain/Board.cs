using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Service;

namespace TicTacToe.Domain
{
    internal class Board : ObservableObject
    {
        #region Fields

        private string _tl = "";
        private string _tm = "";
        private string _tr = "";
        private string _ml = "";
        private string _mm = "";
        private string _mr = "";
        private string _bl = "";
        private string _bm = "";
        private string _br = "";

        #endregion

        #region Properties

        public string TL
        {
            get => _tl;
            set => SetProperty(ref _tl, value);
        }

        public string TM
        {
            get => _tm;
            set => SetProperty(ref _tm, value);
        }

        public string TR
        {
            get => _tr;
            set => SetProperty(ref _tr, value);
        }

        public string ML
        {
            get => _ml;
            set => SetProperty(ref _ml, value);
        }

        public string MM
        {
            get => _mm;
            set => SetProperty(ref _mm, value);
        }

        public string MR
        {
            get => _mr;
            set => SetProperty(ref _mr, value);
        }

        public string BL
        {
            get => _bl;
            set => SetProperty(ref _bl, value);
        }

        public string BM
        {
            get => _bm;
            set => SetProperty(ref _bm, value);
        }

        public string BR
        {
            get => _br;
            set => SetProperty(ref _br, value);
        }

        public string this[ETicTacToePos pos]
        {
            get => Get((int)pos);
            set => Set((int)pos, value);
        }

        public string this[int pos]
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

        private string Get(int pos)
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
                    return null;
            }
        }

        private void Set(int pos, string val)
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
    }
}