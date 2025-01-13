using BO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (string)value == "Add")
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ConvertNullToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ConvertNullToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ConvertFalseToHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value== true)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ConvertISActive : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? ISActive.Active : ISActive.NotActive;
            }

            return ISActive.Both;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           switch((ISActive)value)
            {
                case ISActive.Active:
                    return true;
                case ISActive.NotActive:
                    return false;
                default:
                    return null;
            }
        }
    }

    public class ConvertStatusEditableForIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var status = (Status)value;

            // Depending on the status, decide if it is possible to edit
            switch (status)
            {
                case Status.Open:
                    return true;
                case Status.OpenInRisk:
                    return true;
                case Status.InTreat:
                    return parameter?.ToString() == "maxTime" ? true : false;
                case Status.TreatInRisk:
                    return parameter?.ToString() == "maxTime" ? true : false;
                case Status.Close:
                    return false;
                case Status.Expired:
                    return false;
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // לא נדרשת המרה חזרה
            return null;
        }
    }
         public class ConvertStatusEditableForIsReadOnly : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var status = (Status)value;

            // Depending on the status, decide if it is possible to edit
            switch (status)
            {
                case Status.Open:
                    return false;
                case Status.OpenInRisk:
                    return false;
                case Status.InTreat:
                    return parameter?.ToString() == "maxTime" ? false : true;
                case Status.TreatInRisk:
                    return parameter?.ToString() == "maxTime" ? false : true;
                case Status.Close:
                    return true;
                case Status.Expired:
                    return true;
                default:
                    return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // לא נדרשת המרה חזרה
            return null;
        }
    }
    public class ConvertBoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }


}
