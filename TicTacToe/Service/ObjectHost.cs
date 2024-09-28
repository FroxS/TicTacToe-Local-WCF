using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TicTacToe.Domain;

namespace TicTacToe.Service
{
    public class ObjectHost
    {
        #region Properties

        private static Dictionary<Type, object> _objects;

        public static IMainWindow MainVindow => Get<IMainWindow>();

        public static IGameServieVM GameService => MainVindow.ViewModel;

        public static bool Configured { get; private set; }

        #endregion

        #region Methods

        public static void Setup(IMainWindow mainWindow)
        {
            Configured = true;
            _objects = new Dictionary<Type, object>();
            Set(mainWindow);
        }

        public static void Set<T>(T obj, bool overide = true)
        {
            if (!Configured)
                return;
            Type type = typeof(T);
            if(_objects.ContainsKey(type)) 
            {
                _objects[type] = obj;
            }
            else
            {
                _objects.Add(type, obj);
            }
        }

        public static T Get<T>() 
        {
            object obj = default(T);
            if (!Configured)
                return (T)obj;
            Type typ = typeof(T);
            _objects.TryGetValue(typ, out obj);
            return (T)obj;
        }

        #endregion
    }
}