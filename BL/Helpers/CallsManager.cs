using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class CallsManager
    {
        internal static ObserverManager Observers = new(); //stage 5 

        private static IDal s_dal = Factory.Get; //stage 4

        internal static BO.Call ConvertDOCallToBOCall(DO.Call doCall)
        {
            var assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == doCall.ID);

            return new BO.Call
            {
                ID = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                verbalDescription = doCall.verbalDescription,
                address = doCall.address,
                latitude = doCall.latitude,
                longitude = doCall.longitude,
                openTime = doCall.openTime,
                maxTime = doCall.maxTime,
                statusC = GetCallStatus(doCall),
                CallAssign = assignmentsForCall.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    fullName = s_dal.Volunteer.Read(v => v.ID == a.VolunteerId)?.fullName,
                    startTreatment = a.startTreatment,
                    finishTreatment = a.finishTreatment,
                    finishT = a.finishT.HasValue ? (BO.FinishType)a.finishT : null
                }).ToList()
            };
        }

        internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
        {
            var assignmentsForCall = s_dal.Assignment.ReadAll(A => A.CallId == doCall.ID);
            var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.startTreatment).FirstOrDefault();
            return new BO.CallInList
            {
                ID = lastAssignmentsForCall?.ID,
                CallId = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                openTime = doCall.openTime,
                timeEndCall = doCall.maxTime != null ? doCall.maxTime - s_dal.Config.Clock : null,
                volunteerLast = lastAssignmentsForCall != null ? s_dal.Volunteer.Read(v => v.ID == lastAssignmentsForCall.VolunteerId)?.fullName : null,
                TimeEndTreat = lastAssignmentsForCall?.finishTreatment != null ? lastAssignmentsForCall.finishTreatment - lastAssignmentsForCall.startTreatment : null,
                status = GetCallStatus(doCall),
                numOfAssignments = assignmentsForCall.Count()
            };
        }
        internal static BO.Status GetCallStatus(DO.Call doCall)
        {

            var lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.ID).OrderByDescending(a => a.startTreatment).FirstOrDefault();

            if (lastAssignment == null)
            {
                if (doCall.maxTime != null && doCall.maxTime < s_dal.Config.Clock)
                    return BO.Status.Expired;
                if (IsInRisk(doCall!))
                    return BO.Status.OpenInRisk;
                else return BO.Status.Open;
            }
            if (lastAssignment.finishT == DO.FinishType.Treated)
            {
                return BO.Status.Close;
            }
            if (doCall.maxTime != null && doCall.maxTime < s_dal.Config.Clock)
            {
                return Status.Expired;
            }
            if (lastAssignment.finishT == null)
            {
                if (IsInRisk(doCall!))
                    return BO.Status.TreatInRisk;
                else return BO.Status.InTreat;
            }
            if (IsInRisk(doCall!))
                return BO.Status.OpenInRisk;
            return BO.Status.Open;//default
        }
        internal static void CheckCallLogic(BO.Call call)
        {
            if (call == null)
            {
                throw new BlUpdateCallException("Call object cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(call.address))
            {
                throw new BlUpdateCallException("Address cannot be null or empty.");
            }

            if (call.openTime == default)
            {
                throw new BlUpdateCallException("Opening time must be a valid date.");
            }

            if (call.maxTime <= call.openTime)
            {
                throw new BlUpdateCallException("Max completion time must be later than the opening time.");
            }

            try
            {
                // Validate address and update geolocation data
                double[] coordinates = Tools.GetGeolocationCoordinates(call.address);
                call.latitude = coordinates[0];
                call.longitude = coordinates[1];
                Observers.NotifyItemUpdated(call.ID);

            }
            catch (AdressDoesNotExistException ex)
            {
                throw new BlUpdateCallException("Invalid address: " + ex.Message);
            }
           
        }
        internal static void CheckCallFormat(BO.Call call)
        {
            if (call == null)
            {
                throw new BlUpdateCallException("Call object cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(call.address))
            {
                throw new BlUpdateCallException("Address cannot be null, empty, or consist only of whitespace.");
            }

            // Regex for address format: street (optional number), city, country
            string addressPattern = @"^\s*[\p{L}\s]+(?:\s+\d{1,5})?,\s*[\p{L}\s]+,\s*[\p{L}\s]+$";
            Regex regex = new Regex(addressPattern);

            if (!regex.IsMatch(call.address))
            {
                throw new BlUpdateCallException("Invalid address format. The address must follow the format: street (optional number), city, country.");
            }
        }

        internal static BO.Call ConvertDOCallWithAssignments(DO.Call doCall, IEnumerable<DO.Assignment> assignmentsForCall)
        {
            return new BO.Call
            {
                ID = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                verbalDescription = doCall.verbalDescription,
                address = doCall.address,
                latitude = doCall.latitude,
                longitude = doCall.longitude,
                openTime = doCall.openTime,
                maxTime = doCall.maxTime,
                statusC = GetCallStatus(doCall),
                CallAssign = assignmentsForCall.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    fullName = s_dal.Volunteer.Read(v => v.ID == a.VolunteerId)?.fullName,
                    startTreatment = a.startTreatment,
                    finishTreatment = a.finishTreatment,
                    finishT = a.finishT.HasValue ? (BO.FinishType)a.finishT : null
                }).ToList()
            };
        }
        public static bool IsInRisk(DO.Call call) => call!.maxTime - s_dal.Config.Clock <= s_dal.Config.RiskRange;
        internal static BO.ClosedCallInList ConvertDOCallToBOCloseCallInList(DO.Call doCall, DO.Assignment lastAssignment)
        {
            return new BO.ClosedCallInList
            {
                ID = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                address = doCall.address,
                openTime = doCall.openTime,
                startTreatment = lastAssignment.startTreatment,
                finishTreatment = lastAssignment.finishTreatment,
                finishT = lastAssignment.finishT == null ? throw new Exception("problm there is now finish type ") : (BO.FinishType)lastAssignment.finishT,
            };
        }
        internal static BO.OpenCallInList ConvertDOCallToBOOpenCallInList(DO.Call doCall, int id)
        {
            var vol = s_dal.Volunteer.Read(v => v.ID == id);
            double idLat = vol == null ? 0 : vol.Latitude ?? 0;
            double idLon = vol == null ? 0 : vol.Longitude ?? 0;
            return new BO.OpenCallInList
            {
                ID = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                verbalDescription = doCall.verbalDescription,
                address= doCall.address,
                openTime = doCall.openTime,
                maxTime = doCall.maxTime,
                distance = Tools.CalculateDistance(doCall.latitude, doCall.longitude, idLat, idLon,(BO.Distance)vol.distanceType),
            };
        }
        internal static DO.Call convertFormBOCallToDo(BO.Call boCall)
        {
            DO.Call doCall = new(
             ID: boCall.ID,
             address: boCall.address,
             callT: (DO.CallType)boCall.callT,
             latitude: boCall.latitude,
             longitude: boCall.longitude,
             openTime: boCall.openTime,
             maxTime: boCall.maxTime,
             verbalDescription: boCall.verbalDescription

             );
            return doCall;
        }
        /// <summary>
        /// This function checks if a call has expired based on its max time and whether it has been assigned.
        /// If the call is expired, it handles the assignment status accordingly.
        /// </summary>
        internal static void checkIfExpiredCall()
        {
            IEnumerable<DO.Call> calls = s_dal.Call.ReadAll();
            IEnumerable<BO.Call> boCalls = from dCall in calls
                                           where (dCall.maxTime == null ? true : dCall.maxTime < s_dal.Config.Clock)
                                           select (ConvertDOCallToBOCall(dCall));
            foreach (BO.Call call in boCalls)
            {
                if (call.CallAssign == null|| call.CallAssign.Count==0)
                {
                    s_dal.Assignment.Create(new DO.Assignment(0, call.ID, 0, s_dal.Config.Clock, s_dal.Config.Clock, DO.FinishType.ExpiredCancel));

                    

                }
                else
                {
                    var lastAss = call.CallAssign.OrderByDescending(a => a.startTreatment).First();
                    if (lastAss.finishT == null)
                    {
                        var assing = s_dal.Assignment.Read(a => a.VolunteerId == lastAss.VolunteerId && a.finishTreatment == null && a.finishT == null);
                        s_dal.Assignment.Update(new DO.Assignment(assing.ID, assing.VolunteerId, lastAss.VolunteerId, lastAss.startTreatment, s_dal.Config.Clock, DO.FinishType.ExpiredCancel));
                        VolunteersManager.Observers.NotifyItemUpdated(assing.VolunteerId);
                        VolunteersManager.Observers.NotifyListUpdated();

                    }


                }
                CallsManager.Observers.NotifyItemUpdated(call.ID);  //stage 5
                CallsManager.Observers.NotifyListUpdated();


            }

        }
    }

}
