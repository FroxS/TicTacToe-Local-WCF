using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TicTacToe.Conventers
{
    /// <summary>
    /// Base value conventer that allows direct XMAL usage
    /// </summary>
    /// <typeparam name="T">The type of this value conventer </typeparam>
    public abstract class BaseValueConventer<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        #region Private members
        /// <summary>
        /// Silngle static instance of this value
        /// </summary>
        private static T mConventer = null;

        #endregion

        /// <summary>
        /// Providers a static instance of the value conventer
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return mConventer ?? (mConventer = new T());
        }

        #region Value Conventer Methods
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}