using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TicTacToe.Service
{
    public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public event PropertyChangingEventHandler PropertyChanging = (sender, e) => { };

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnPropertiesChanged(string[] names = null)
        {
            if (names != null)
                names.ToList().ForEach((o) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(o)));
        }

        protected void OnPropertyChanged(params string[] names)
        {
            if (names != null)
                names.ToList().ForEach((o) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(o)));
        }

        protected void OnPropertyChanging([CallerMemberName] string name = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }

        protected void NotifyPropChanged(params string[] propsName)
        {
            foreach (string prop in propsName)
            {
                if (string.IsNullOrEmpty(prop)) continue;
                OnPropertyChanged(prop);
            }
        }

        protected void SetProperty<T>(
            Expression<Func<T>> field,
            T value,
            Action onChanged = null)
        {
            Func<T> compiledExpression = field.Compile();
            T result = compiledExpression();
            result = value;
            if (field.Body is MemberExpression memberExpression)
            {
                OnPropertyChanged(memberExpression.Member.Name);
            }
            onChanged?.Invoke();
        }

        protected void U<T>(
            Expression<Func<T>> field)
        {
            if (field.Body is MemberExpression memberExpression)
            {
                OnPropertyChanged(memberExpression.Member.Name);
            }
        }

        protected void SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string propName = null,
            Action onChanged = null)
        {
            field = value;
            OnPropertyChanged(propName);
            onChanged?.Invoke();
        }

        protected void SetPropertyOut<T>(
            out T field,
            T value,
            [CallerMemberName] string propName = null,
            Action onChanged = null)
        {
            field = value;
            OnPropertyChanged(propName);
            onChanged?.Invoke();
        }

        protected void SetPropertyVal<T>(
            T field,
            object value,
            [CallerMemberName] string propName = null,
            Action onChanged = null)
        {
            if (propName == null)
                throw new ArgumentException("Set propert name");

            PropertyInfo property = typeof(T).GetProperty(propName);

            if (property == null)
                throw new ArgumentException($"Property {propName} not found in object {field.ToString()}");
            property.SetValue(field, value);
            OnPropertyChanged(propName);
            onChanged?.Invoke();
        }
    }
}