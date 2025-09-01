using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ((string)value == "Add" || (string)value == ""))
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
            if ((bool)value == true)
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
            switch ((ISActive)value)
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
            throw new NotImplementedException();
        }
    }
        public class ConvertBoolToVisibility : IValueConverter
        {
            // הפניה לשכבת BL
            static readonly BlApi.IBl _bl = BlApi.Factory.Get();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // בדיקה אם הערך הוא מזהה קריאה (callId)
                if (value is int callId)
                {
                    try
                    {
                        // קריאה לפונקציה CanDeleteCall דרך BL
                        bool canDelete = _bl.Call.CanDeleteCall(callId);

                        // החזרת Visibility בהתאם לתוצאה
                        return canDelete ? Visibility.Visible : Visibility.Collapsed;
                    }
                    catch
                    {
                        // במקרה של חריגה, מחזיר Collapsed
                        return Visibility.Collapsed;
                    }
                }

                // במקרה של ערך לא תקין, מחזיר Collapsed
                return Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.Open => Brushes.Green,
                    Status.InTreat => Brushes.Blue,
                    Status.Close => Brushes.Gray,
                    Status.Expired => Brushes.Red,
                    Status.OpenInRisk => Brushes.Orange,
                    Status.TreatInRisk => Brushes.Purple,
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class FinishTypesToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FinishType finishType)
            {
                return finishType switch
                {
                    FinishType.ManagerCancel => Brushes.Green,
                   FinishType.SelfCancel => Brushes.Blue,
                    FinishType.Treated => Brushes.Gray,
                    FinishType.ExpiredCancel => Brushes.Red,

                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CallTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    CallType.BabyGift => Brushes.DeepPink,
                    CallType.MomGift => Brushes.LightBlue,
                    CallType.HouseholdHelp => Brushes.LightGreen,
                    CallType.MealPreparation => Brushes.DarkTurquoise,
                    CallType.None => Brushes.DarkGray,
                    _ => Brushes.Brown
                };
            }
            return Brushes.Brown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CallTypeToColorBackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    CallType.BabyGift => Brushes.Pink,
                    CallType.MomGift => Brushes.Blue,
                    CallType.HouseholdHelp => Brushes.Green,
                    CallType.MealPreparation => Brushes.Turquoise,
                    CallType.None => Brushes.Gray,
                    _ => Brushes.Beige
                };
            }
            return Brushes.Brown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DeleteVolunteerVisbilty : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = (int)value;

            if (s_bl.Volunteer.CanDeleteVoluenteer(id)) 
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

    public class ConvertTextSimulatorState: IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           if(value != null&& value is bool)
            {
                if ((bool)value == false)
                    return "start Simulator";
                return "stop Simulator";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertBoolToNotBool : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return !(bool)value;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConverterThreeValueNullToColor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return Brushes.White;

            object firstValue = values[0];
            object secondValue = values[1];
            object thirdValue = values[2];

            if (firstValue != null && (secondValue == null || thirdValue == null))
            {
                return Brushes.Red;
            }

            return Brushes.White ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }

}


