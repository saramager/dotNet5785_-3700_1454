using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class VolunteersManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        internal static BO.VolunteerInList convertDOToBOInList(DO.Volunteer doVolunteer)
        {
            var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            int sumCalls = call.Count(ass => ass.finishT == DO.FinishType.Treated);
            int sumCanceld = call.Count(ass => ass.finishT == DO.FinishType.SelfCancel);
            int sumExpired = call.Count(ass => ass.finishT == DO.FinishType.ExpiredCancel);
            int? idCall = call.Count(ass => ass.finishTreatment == null);
            return new()
            {
                ID = doVolunteer.ID,
                fullName = doVolunteer.fullName,
                active = doVolunteer.active,
                numCallsHandled = sumCalls,
                numCallsCancelled = sumCanceld,
                numCallsExpired = sumExpired,
                CallId = idCall,
            };
        }
        internal static BO.Volunteer convertDOToBOVolunteer(DO.Volunteer doVolunteer)
        {
            var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            int sumCalls = call.Count(ass => ass.finishT == DO.FinishType.Treated);
            int sumCanceld = call.Count(ass => ass.finishT == DO.FinishType.SelfCancel);
            int sumExpired = call.Count(ass => ass.finishT == DO.FinishType.ExpiredCancel);
            int? idCall = call.Count(ass => ass.finishTreatment == null);
            CallInProgress? c = GetCallIn(doVolunteer);
            return new()
            {
                Id = doVolunteer.ID,
                fullName = doVolunteer.fullName,
                phone = doVolunteer.phone,
                email = doVolunteer.email,
                password = doVolunteer.password,
                currentAddress = doVolunteer.currentAddress,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                role = (BO.RoleType)doVolunteer.role,
                active = doVolunteer.active,
                numCallsHandled = sumCalls,
                numCallsCancelled = sumCanceld,
                numCallsExpired = sumExpired,
                maxDistance = doVolunteer.maxDistance,
                distanceType = (BO.Distance)doVolunteer.distanceType
            };
        }
        internal static BO.CallInProgress? GetCallIn(DO.Volunteer doVolunteer)
        {

            var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            DO.Assignment? assignmentTreat = call.Find(ass => ass.finishTreatment == null);
            if (assignmentTreat != null)
            {
                DO.Call? callTreat = s_dal.Call.Read(c => c.ID == assignmentTreat.CallId);
                if (callTreat != null)
                    return new()
                    {
                        ID = assignmentTreat.ID,
                        CallId = assignmentTreat.CallId,
                        callT = (BO.CallType)callTreat.callT,
                        verbalDescription = callTreat.verbalDescription,
                        address = callTreat.address,
                        openTime = callTreat.openTime,
                        maxTime = callTreat.maxTime,
                        startTreatment = assignmentTreat.startTreatment,
                        //CallDistance =/* Tools/*.CalculateDistance*/ (callTreat.latitude, callTreat.longitude, doVolunteer.Latitude, doVolunteer.Longitude),
                        //statusT = (callTreat.MaxTimeToClose - ClockManager.Now <= getRange()  ? BO.StatusTreat.RiskOpen : BO.StatusTreat.Treat,
                    };
            }
            return null;
        }

        internal static bool checkeVolunteer(BO.Volunteer volunteer, BO.RoleType role = RoleType.Manager)
        {
            if (string.IsNullOrEmpty(volunteer.email) || volunteer.email.Count(c => c == '@') != 1)
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+\.com$";

            if (!Regex.IsMatch(volunteer.email, pattern))
            {
                return false;
            }
            if (!(IsValidId(volunteer.Id)))
                return false;

            // add adress
            return true;


        }
        internal static bool IsValidId(long id)
        {
            // בדיקה אם תעודת הזהות היא מספר בן 9 ספרות
            if (id < 100000000 || id > 999999999)
            {
                return false;  // לא תקין אם זה לא בדיוק 9 ספרות
            }

            // חישוב ספרת הביקורת
            int sum = 0;
            for (int i = 0; i < 8; i++)  // רק עבור 8 הספרות הראשונות
            {
                int digit = (int)(id % 10);  // קבלת הספרה הנוכחית (הספרה האחרונה)
                id /= 10;  // הסרת הספרה הנוכחית

                if (i % 2 == 0)  // אם המיקום אי זוגי
                {
                    sum += digit;  // פשוט מוסיפים את הספרה
                }
                else  // אם המיקום זוגי
                {
                    int doubled = digit * 2;
                    sum += doubled > 9 ? doubled - 9 : doubled;  // אם המכפלה גדולה מ-9, מחסירים 9
                }
            }

            // חישוב ספרת הביקורת
            int checkDigit = (10 - (sum % 10)) % 10;  // ספרת הביקורת

            // השוואה לספרת הביקורת
            return checkDigit == (int)(id % 10);  // בדוק אם הספרה האחרונה תואמת
        } 
    internal static DO.Volunteer convertFormBOVolunteerToDo(BO.Volunteer BoVolunteer)
        {
           DO.Volunteer doVl = new (
                ID: BoVolunteer.Id,
                fullName: BoVolunteer.fullName,
                 phone: BoVolunteer.phone,
                email: BoVolunteer.email,
                active: BoVolunteer.active,
                role: (DO.RoleType)BoVolunteer.role,
               distanceType: (DO.Distance)BoVolunteer.distanceType,
              password: BoVolunteer.password,
              currentAddress: BoVolunteer.currentAddress,
             Latitude: BoVolunteer.Latitude,
             Longitude: BoVolunteer.Longitude,
            maxDistance: BoVolunteer.maxDistance
           
                       );
            return doVl;
        }
    } }

