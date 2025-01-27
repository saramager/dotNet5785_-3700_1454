using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class CallsManager
    {
        internal static ObserverManager Observers = new(); //stage 5 

        private static IDal s_dal = DalApi.Factory.Get; //stage 4

        internal static BO.Call ConvertDOCallToBOCall(DO.Call doCall)
        {
            IEnumerable<DO.Assignment> assignmentsForCall;
          List<BO.CallAssignInList> callAss;
            lock (AdminManager.BlMutex)
            {
                assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == doCall.ID);
                callAss = assignmentsForCall.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    fullName = s_dal.Volunteer.Read(v => v.ID == a.VolunteerId)?.fullName ?? string.Empty,
                    startTreatment = a.startTreatment,
                    finishTreatment = a.finishTreatment,
                    finishT = a.finishT.HasValue ? (BO.FinishType)a.finishT : null
                }).ToList();
            }
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
               CallAssign= callAss
            };
        }

        internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
        {
            IEnumerable<Assignment>? assignmentsForCall;
            lock (AdminManager.BlMutex)
                assignmentsForCall = s_dal.Assignment.ReadAll(A => A.CallId == doCall.ID);
            var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.ID).FirstOrDefault();
            lock (AdminManager.BlMutex)
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
           DO.Assignment? lastAssignment;
            DateTime now;
            lock (AdminManager.BlMutex)
            {
                lastAssignment = s_dal.Assignment.ReadAll(ass => ass.CallId == doCall.ID).OrderByDescending(a => a.ID).FirstOrDefault();
                now = s_dal.Config.Clock;

            }

            if (lastAssignment == null)
            {
                if (doCall.maxTime != null && doCall.maxTime < now)
                    return BO.Status.Expired;
                if (IsInRisk(doCall!))
                    return BO.Status.OpenInRisk;
                else return BO.Status.Open;
            }
            if (lastAssignment.finishT == DO.FinishType.Treated)
            {
                return BO.Status.Close;
            }
            if (doCall.maxTime != null && doCall.maxTime <  now)
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
            // add for adress
        }

        internal static BO.Call ConvertDOCallWithAssignments(DO.Call doCall, IEnumerable<DO.Assignment> assignmentsForCall)
        {
            BO.Status status;
            lock (AdminManager.BlMutex)
            {
                status = GetCallStatus(doCall);



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
                    statusC = status,
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
            DO.Volunteer vol;
            lock (AdminManager.BlMutex)
            {
                vol = s_dal.Volunteer.Read(v => v.ID == id)!;
            }
            double? idLat = vol?.Latitude;
            double? idLon = vol?.Longitude;
            double? callLat = doCall.latitude;
            double? callLon = doCall.longitude;
            double dis = 0;
            if (idLat.HasValue && idLon.HasValue&& callLat.HasValue && callLon.HasValue) 
                dis = Tools.CalculateDistance(callLat.Value, callLon.Value, idLat.Value, idLon.Value, (BO.Distance)vol.distanceType);
            //double? idLat = vol == null ? null : vol.Latitude ?? null;
            //double? idLon = vol == null ? null : vol.Longitude ?? null;

            //double dis=0;
            //if (idLon != null && idLat != null)
            //    dis = Tools.CalculateDistance(doCall.latitude, doCall.longitude, idLat!, idLon!, (BO.Distance)vol.distanceType);

            return new BO.OpenCallInList
            {
                ID = doCall.ID,
                callT = (BO.CallType)doCall.callT,
                verbalDescription = doCall.verbalDescription,
                address= doCall.address,
                openTime = doCall.openTime,
                maxTime = doCall.maxTime,
                distance = dis,
            };
        }
        internal static DO.Call convertFormBOCallToDo(BO.Call boCall)
        {
            DO.Call doCall = new(
             ID: boCall.ID,
             address: boCall.address,
             callT: (DO.CallType)boCall.callT,
             latitude:null,
             longitude: null,
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
        private static int s_periodicCounter = 0;
        internal static void checkIfExpiredCall(DateTime oldClock, DateTime newClock)
        {
            Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}"; //stage 7 (optional)
            IEnumerable<BO.Call> boCalls;
            List<int> idSThatChanges= new List<int>();

            lock (AdminManager.BlMutex)
            {
                IEnumerable<DO.Call> calls = s_dal.Call.ReadAll();
                boCalls = from dCall in calls
                          where (dCall.maxTime == null ? true : dCall.maxTime < newClock)
                          select (ConvertDOCallToBOCall(dCall));
                boCalls = boCalls.ToList();
            }
            foreach (BO.Call call in boCalls)
            {
                if (call.CallAssign == null|| call.CallAssign.Count==0)
                {
                    s_dal.Assignment.Create(new DO.Assignment(0, call.ID, 0, s_dal.Config.Clock, s_dal.Config.Clock, DO.FinishType.ExpiredCancel));

                    

                }
                else
                {
                    var lastAss = call.CallAssign.OrderByDescending(a=>a.startTreatment).First();
                    if (lastAss.finishT == null)
                    {
                        DO.Assignment? assing;
                        DateTime clock;
                        lock (AdminManager.BlMutex)
                        {
                            assing = s_dal.Assignment.Read(a => a.VolunteerId == lastAss.VolunteerId && a.finishTreatment == null && a.finishT == null); 
                            clock= s_dal.Config.Clock;
                        }
                        s_dal.Assignment.Update(new DO.Assignment(assing.ID, assing.VolunteerId, lastAss.VolunteerId, lastAss.startTreatment, newClock, DO.FinishType.ExpiredCancel));
                        idSThatChanges.Add(assing.VolunteerId);

                    }


                }

               

            }
            foreach (int id in  idSThatChanges)
                VolunteersManager.Observers.NotifyItemUpdated(id);
          if (idSThatChanges.Count!=0)
                VolunteersManager.Observers.NotifyListUpdated();
          foreach (var  calll in boCalls)
                CallsManager.Observers.NotifyItemUpdated(calll.ID);  //stage 5
            if (boCalls.Count()!= 0)
                CallsManager.Observers.NotifyListUpdated();





        }
        internal static async Task updateCoordinatesForCallAddressAsync(DO.Call doCall)
        {
            if (doCall.address is not null)
            {
                double[] loctions = await Tools.GetGeolocationCoordinatesAsync(doCall.address);
                if (loctions is not null)
                {
                    doCall = doCall with { latitude = loctions[0], longitude = loctions[1] };
                    lock (AdminManager.BlMutex)
                        s_dal.Call.Update(doCall);
                    Observers.NotifyListUpdated();
                    Observers.NotifyItemUpdated(doCall.ID);
                }
            }
        }
    }

}
