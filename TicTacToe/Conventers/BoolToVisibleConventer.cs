using System;
using System.Globalization;
using System.Windows;

namespace TicTacToe.Conventers
{
    public class BoolToVisibleConventer : BaseValueConventer<BoolToVisibleConventer>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if(parameter is bool)
                invert = (bool)parameter;
            
            if(value is bool flag) 
            {
                if(flag) 
                    return invert ? Visibility.Collapsed : Visibility.Visible;
                else
                    return invert ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter is bool)
                invert = (bool)parameter;

            if (value is Visibility vis)
            {
                if (vis == Visibility.Visible)
                    return invert ? false: true;
                else
                    return invert ? true : false;
            }

            return true;
        }
    }
}