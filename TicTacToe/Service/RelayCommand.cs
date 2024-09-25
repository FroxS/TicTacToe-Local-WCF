using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TicTacToe.Service
{
    internal class RelayCommand<T> : ICommand
    {
        #region Private Properties

        protected readonly Predicate<T> _canExecute;
        protected readonly Action<T> _execute;

        #endregion

        #region Helpers

        internal static Action<Exception> DefaultActionOnError { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
            _execute = execute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            try
            {
                return _canExecute == null || _canExecute((T)parameter);
            }
            catch (Exception ex)
            {
                DefaultActionOnError?.Invoke(ex);
                return false;
            }

        }

        public virtual void Execute(object parameter)
        {
            //try
            //{
            _execute((T)parameter);
            //}
            //catch (Exception ex)
            //{
            //    DefaultActionOnError?.Invoke(ex);
            //}

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    internal class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base((o) => execute(), (o) => true)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : base((o) => execute(), (o) => canExecute())
        {

        }
    }

    internal class AsyncRelayCommand : ICommand
    {
        #region Private Properties

        private readonly Action<Exception> _onException;

        private bool _isExecuting;

        private readonly Func<Task> _callback;

        protected readonly Predicate<object> _canExecute;

        #endregion

        #region Public properties

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        public AsyncRelayCommand(Func<Task> callback, Action<Exception> onException = null)
        {
            _onException = onException;
            _callback = callback;
        }

        public AsyncRelayCommand(Func<Task> callback, Predicate<object> canExecute, Action<Exception> onException = null)
        {
            _onException = onException;
            _callback = callback;
            _canExecute = canExecute;
        }

        #endregion

        #region Private methods

        protected async Task ExecuteAsync(object parameter)
        {
            await _callback();
        }

        #endregion

        #region Public methods

        public bool CanExecute(object parameter)
        {
            try
            {
                return !IsExecuting && (_canExecute == null || _canExecute.Invoke(parameter));

            }
            catch (Exception ex)
            {
                if (_onException != null)
                    _onException?.Invoke(ex);
                else
                    RelayCommand.DefaultActionOnError?.Invoke(ex);

                return false;
            }

        }

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                if (_onException != null)
                    _onException?.Invoke(ex);
                else
                    RelayCommand.DefaultActionOnError?.Invoke(ex);
            }

            IsExecuting = false;
        }

        #endregion

    }

    internal class AsyncRelayCommand<T> : ICommand
    {
        #region Private Properties

        protected readonly Predicate<T> _canExecute;

        private readonly Action<Exception> _onException;

        private bool _isExecuting;

        private readonly Func<T, Task> _callback;

        #endregion

        #region Public properties

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        public AsyncRelayCommand(Func<T, Task> callback, Predicate<T> canExecute = null, Action<Exception> onException = null)
        {
            _onException = onException;
            _callback = callback;
        }

        #endregion

        #region Private methods

        protected async Task ExecuteAsync(object parameter)
        {
            try
            {
                if (parameter is T par)
                    await _callback(par);
                else
                    await _callback(default(T));
            }
            catch (Exception ex)
            {
                if (_onException != null)
                    _onException?.Invoke(ex);
                else
                    RelayCommand.DefaultActionOnError?.Invoke(ex);
            }

        }

        #endregion

        #region Public methods

        public bool CanExecute(object parameter) => !IsExecuting;

        public async void Execute(object parameter)
        {
            IsExecuting = true;

            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                if (_onException != null)
                    _onException?.Invoke(ex);
                else
                    RelayCommand.DefaultActionOnError?.Invoke(ex);
            }

            IsExecuting = false;
        }

        #endregion

    }
}