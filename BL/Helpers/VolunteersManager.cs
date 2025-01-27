using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class VolunteersManager
    {
        internal static ObserverManager Observers = new(); //stage 5 

        private static IDal s_dal = DalApi.Factory.Get; //stage 4
        /// <summary>
        /// convert volunteer form Do to volunteerINList  from BO
        /// </summary>
        /// <param name="doVolunteer"> get the volunteer to return </param>
        /// <returns> the BO volunteerInList  </returns>
        internal static BO.VolunteerInList convertDOToBOInList(DO.Volunteer doVolunteer)
        {
            List<DO.Assignment>? calls;
            lock (AdminManager.BlMutex)  //stage 7
                calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            int sumCalls = calls.Count(ass => ass.finishT == DO.FinishType.Treated);
            int sumCanceld = calls.Count(ass => ass.finishT == DO.FinishType.SelfCancel);
            int sumExpired = calls.Count(ass => ass.finishT == DO.FinishType.ExpiredCancel);
            var call = calls.Find(ass => ass.finishT == null);
            DO.Call? callD;
            lock (AdminManager.BlMutex)  //stage 7
                callD = call == null ? null : s_dal.Call.Read(c => c.ID == call.CallId);
            return new()
            {
                ID = doVolunteer.ID,
                fullName = doVolunteer.fullName,
                active = doVolunteer.active,
                numCallsHandled = sumCalls,
                numCallsCancelled = sumCanceld,
                numCallsExpired = sumExpired,
                callT = callD == null ? BO.CallType.None : (BO.CallType)callD.callT
            };
        }
        /// <summary>
        /// get volunteer from do and convert it to Bo volunteer 
        /// </summary>
        /// <param name="doVolunteer"> the Dovolunteer </param>
        /// <returns>the bo vlounteer </returns>
        internal static BO.Volunteer convertDOToBOVolunteer(DO.Volunteer doVolunteer)
        {
            List<DO.Assignment>? call;
            lock (AdminManager.BlMutex)  //stage 7
                call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            int sumCalls = call.Count(ass => ass.finishT == DO.FinishType.Treated);
            int sumCanceld = call.Count(ass => ass.finishT == DO.FinishType.SelfCancel);
            int sumExpired = call.Count(ass => ass.finishT == DO.FinishType.ExpiredCancel);
            //int? idCall = call.Find(ass => ass.finishTreatment == null&&ass.finishT==null);
            CallInProgress? c = GetCallIn(doVolunteer);
            return new()
            {
                Id = doVolunteer.ID,
                fullName = doVolunteer.fullName,
                phone = doVolunteer.phone,
                email = doVolunteer.email,
                password = doVolunteer.password != null ? Decrypt(doVolunteer.password) : null,
                currentAddress = doVolunteer.currentAddress,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                role = (BO.RoleType)doVolunteer.role,
                active = doVolunteer.active,
                numCallsHandled = sumCalls,
                numCallsCancelled = sumCanceld,
                numCallsExpired = sumExpired,
                maxDistance = doVolunteer.maxDistance,
                distanceType = (BO.Distance)doVolunteer.distanceType,
                callProgress = c
            };
        }
        /// <summary>
        /// get volunteer and return Call in progress if there is one 
        /// </summary>
        /// <param name="doVolunteer"> the volunteer we wnat to check if there is </param>
        /// <returns>callin progerss th this spsifiec volunteer </returns>
        internal static BO.CallInProgress? GetCallIn(DO.Volunteer doVolunteer)
        {
            List<DO.Assignment>? call;
            lock (AdminManager.BlMutex)  //stage 7
                call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            DO.Assignment? assignmentTreat = call.Find(ass => ass.finishT == null);

            DO.Call? callTreat;
            if (assignmentTreat != null)
            {
                lock (AdminManager.BlMutex)  //stage 7
                    callTreat = s_dal.Call.Read(c => c.ID == assignmentTreat.CallId);

                if (callTreat != null)
                {
                    double distanceOfCall = 0;
                    if (doVolunteer.Latitude != null && doVolunteer.Longitude != null)
                    {
                        distanceOfCall = 0;
                        if (callTreat.latitude != null && callTreat.longitude != null)

                            Tools.CalculateDistance
                        (
                          (double)callTreat.latitude,
                          (double)callTreat.longitude,
                          (double)doVolunteer.Latitude,
                          (double)doVolunteer.Longitude,
                          (BO.Distance)doVolunteer.distanceType
                        );
                    }
                    lock (AdminManager.BlMutex)  //stage 7
                    {
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
                            CallDistance = distanceOfCall,
                            //CallDistance = Tools.CalculateDistance(callTreat.latitude, callTreat.longitude, latitude, longitude,(BO.Distance)doVolunteer.distanceType),
                            statusT = (callTreat.maxTime - AdminManager.Now <= s_dal.Config.RiskRange ? BO.Status.TreatInRisk : BO.Status.InTreat),
                        };
                    }
                }
            }
            return null;
        }
        /// /// <summary>
        /// This function checks the format of a volunteer's email, phone, and max distance.
        /// It validates the email format (must be in a basic email format with '@' and domain),
        /// checks if the phone number is numeric and within a certain length, and ensures 
        /// that the maximum distance is a non-negative value.
        /// </summary>
        /// <param name="volunteer">The volunteer object containing the email, phone, and max distance fields to validate.</param>
        internal static void checkeVolunteerFormat(BO.Volunteer volunteer)
        {
            if (string.IsNullOrEmpty(volunteer.email) || volunteer.email.Count(c => c == '@') != 1)
            {
                throw new BO.EmailDoesNotcoretct($"email :{volunteer.email} have problem with format ");
            }

            //string pattern = @"^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+\.com$";
            string pattern = @"^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+$";

            if (!Regex.IsMatch(volunteer.email, pattern))
            {
                throw new BO.EmailDoesNotcoretct($"email :{volunteer.email} have problem with format ");
            }
            if (string.IsNullOrEmpty(volunteer.phone) || volunteer.phone.Length < 9 || volunteer.phone.Length > 10 || !(volunteer.phone.All(char.IsDigit)))
                throw new BO.PhoneDoesNotcoretct($"phone :{volunteer.phone} have problem with format ");

            if (volunteer.maxDistance < 0)
                throw new MaxDistanceDoesNotcoretct("Max Distance can't be negavite ");
        }
        /// <summary>
        /// This function checks the validity of a volunteer's ID and password.
        /// It ensures that the ID is valid (based on a custom validation function) and 
        /// that the password meets the requirements: at least 6 characters long, 
        /// contains an uppercase letter, and includes a digit.
        /// </summary>
        /// <param name="volunteer">The volunteer object that contains the ID and password to be validated.</param>
        internal static void checkeVolunteerlogic(BO.Volunteer volunteer)
        {
            if (!(IsValidId(volunteer.Id)))
                throw new BO.IdDoesNotVaildException("the id isnt valid ");
            if (volunteer != null && !IsStrongPassword(volunteer.password!))
                throw new BO.PaswordDoesNotstrongException($" this password :{volunteer.password!} dont have at least 6 characters, contains an uppercase letter and a digit");
        }
        /// <summary>
        /// Validates an Israeli 9-digit ID using the Luhn algorithm.
        /// </summary>
        /// <param name="id">The ID number to validate.</param>
        /// <returns>True if the ID is valid, false otherwise.</returns>
        /// <remarks>
        /// This code was written with the assistance of ChatGPT, a language model developed by OpenAI.
        /// </remarks>
        internal static bool IsValidId(long id)
        {
            // Check if ID is exactly 9 digits.
            if (id < 100000000 || id > 999999999)
            {
                return false;
            }

            // Luhn algorithm to calculate checksum for first 8 digits.
            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int digit = (int)(id % 10);
                id /= 10;

                if (i % 2 == 0)
                {
                    sum += digit;  // Odd index: add digit.
                }
                else
                {
                    int doubled = digit * 2;
                    sum += doubled > 9 ? doubled - 9 : doubled;  // Even index: subtract 9 if doubled > 9.
                }
            }

            // Calculate and compare checksum.
            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == (int)(id % 10);  // Valid if checksum matches.
        }
        /// <summary>
        /// Converts a BO.Volunteer object to a DO.Volunteer object.
        /// This method performs the following operations:
        /// - Maps the properties from BO.Volunteer to DO.Volunteer.
        /// - Encrypts the password if present.
        /// - Doesnt treat coordinates
        /// </summary>
        /// <param name="BoVolunteer">The BO.Volunteer object that contains the volunteer data to be converted.</param>
        /// <returns>A DO.Volunteer object populated with the data from the BO.Volunteer object.</returns>
        internal static DO.Volunteer convertFormBOVolunteerToDo(BO.Volunteer BoVolunteer)
        {

            //if (BoVolunteer.currentAddress != null && BoVolunteer.currentAddress != "")
            //{
            //    double[] cordintes = Tools.GetGeolocationCoordinates(BoVolunteer.currentAddress);
            //    BoVolunteer.Latitude = cordintes[0];
            //    BoVolunteer.Longitude = cordintes[1];
            //}
            //else
            //{
            //    BoVolunteer.Latitude = null;
            //    BoVolunteer.Longitude = null;
            //    BoVolunteer.currentAddress = null;
            //}
            DO.Volunteer doVl = new
                (
                 ID: BoVolunteer.Id,
                 fullName: BoVolunteer.fullName,
                  phone: BoVolunteer.phone,
                 email: BoVolunteer.email,
                 active: BoVolunteer.active,
                 role: (DO.RoleType)BoVolunteer.role,
                distanceType: (DO.Distance)BoVolunteer.distanceType,
               password: BoVolunteer.password != null ? Encrypt(BoVolunteer.password) : null,
               currentAddress: BoVolunteer.currentAddress,
            null, null,
             maxDistance: BoVolunteer.maxDistance

                        );
            Observers.NotifyItemUpdated(doVl.ID);//stage 5
            Observers.NotifyListUpdated(); //stage 5

            return doVl;
        }
        // A constant shift value for encryption and decryption (simple Caesar Cipher)
        private static readonly int shift = 3;
        /// <summary>
        /// Encrypts the given password by shifting each character's ASCII value by the specified shift.
        /// </summary>
        /// <param name="password">The password to be encrypted.</param>
        /// <returns>The encrypted password as a string.</returns>
        /// <remarks>
        /// This function was created with the assistance of ChatGPT, a language model developed by OpenAI.
        /// </remarks>
        public static string Encrypt(string password)
        {
            // Convert the password string into a character array for manipulation
            char[] buffer = password.ToCharArray();

            // Loop through each character in the password
            for (int i = 0; i < buffer.Length; i++)
            {
                // Get the current character
                char c = buffer[i];

                // Shift the ASCII value of the character by the constant shift value
                buffer[i] = (char)((int)c + shift);
            }

            // Return the encrypted password as a new string
            return new string(buffer);
        }

        /// <summary>
        /// Decrypts the given encrypted password by reversing the encryption process.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password to be decrypted.</param>
        /// <returns>The decrypted (original) password as a string.</returns>
        /// <remarks>
        /// This function was created with the assistance of ChatGPT, a language model developed by OpenAI.
        /// </remarks>
        public static string Decrypt(string encryptedPassword)
        {
            // Convert the encrypted password string into a character array for manipulation
            char[] buffer = encryptedPassword.ToCharArray();

            // Loop through each character in the encrypted password
            for (int i = 0; i < buffer.Length; i++)
            {
                // Get the current character
                char c = buffer[i];

                // Reverse the shift of the ASCII value by subtracting the constant shift value
                buffer[i] = (char)((int)c - shift);
            }

            // Return the decrypted password as a new string
            return new string(buffer);
        }
        /// <summary>
        /// Checks if the password is at least 6 characters, contains an uppercase letter and a digit.
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <returns>true if strong, false otherwise</returns>
        /// <remarks>Written with the help of ChatGPT (https://openai.com)</remarks>
        static bool IsStrongPassword(string password)
        {
            // Must be at least 6 characters
            if (password.Length < 6) return false;

            // Must contain at least one uppercase letter and one digit
            return password.Any(char.IsUpper) && password.Any(char.IsDigit);
        }

        internal static async Task updateCoordinatesForStudentAddressAsync(DO.Volunteer doVolunteer)
        {
            if (doVolunteer.currentAddress is not null)
            {
                double[] loctions = await Tools.GetGeolocationCoordinatesAsync(doVolunteer.currentAddress);
                if (loctions is not null)
                {
                    doVolunteer = doVolunteer with { Latitude = loctions[0], Longitude = loctions[1] };
                    lock (AdminManager.BlMutex)
                        s_dal.Volunteer.Update(doVolunteer);
                    Observers.NotifyListUpdated();
                    Observers.NotifyItemUpdated(doVolunteer.ID);
                }
            }
        }

        private static readonly Random s_rand = new();
        private static int s_simulatorCounter = 0;

        internal static void SimulateVolunteerActivity()
        {
            Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";
            LinkedList<int> volunteersToUpdate = new();
            List<DO.Volunteer> doVoluList;

            lock (AdminManager.BlMutex)
                doVoluList = s_dal.Volunteer.ReadAll(v => v.active == true).ToList();

            foreach (var doVolunteer in doVoluList)
            {
                int volunteerId = doVolunteer.ID;

                lock (AdminManager.BlMutex)
                {
                    BO.Volunteer volunteer = convertDOToBOVolunteer(doVolunteer);

                    if (volunteer.callProgress == null) // אין קריאה בטיפול
                    {
                        //  קריאה לרשימת הקריאות הפתוחות עם הקואורדינטות המתאימות 
                        IEnumerable<BO.OpenCallInList> openCalls = ReadOpenCallsVolunteerHelp(volunteerId);

                        // בחירה רנדומלית עם הסתברות
                        if (openCalls.Any() && s_rand.NextDouble() <= 0.2) // הסתברות 20%
                        {
                            var selectedCall = openCalls.ElementAt(s_rand.Next(openCalls.Count()));

                            lock (AdminManager.BlMutex)
                            {
                                ChooseCallTreatHelp(volunteer.Id, selectedCall.ID);//
                                volunteersToUpdate.AddLast(volunteerId);
                            }
                        }
                    }
                    else // יש קריאה בטיפול
                    {
                        BO.CallInProgress activeCall = volunteer.callProgress;

                        

                        if (treatmentDuration.TotalMinutes >= distance / 2 + s_rand.Next(5, 15)) // מספיק זמן לטיפול
                        {
                            try
                            {
                                lock (AdminManager.BlMutex)
                                {
                                    FinishTreatHelp(volunteerId, activeCall.ID);
                                    volunteersToUpdate.AddLast(volunteerId);
                                }
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                // טיפול בחריגה במקרה של בעיה במתודה FinishTreat
                                Console.WriteLine($"Error finishing treatment: {ex.Message}");
                            }
                        }
                        else if (s_rand.NextDouble() <= 0.1) // הסתברות 10% לביטול
                        {
                            try
                            {
                                lock (AdminManager.BlMutex)
                                {
                                    cancelTreatHelp(volunteerId, activeCall.ID);
                                    volunteersToUpdate.AddLast(volunteerId);
                                }
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                // טיפול בחריגה במקרה של בעיה במתודה cancelTreat
                                Console.WriteLine($"Error canceling treatment: {ex.Message}");
                            }
                        }
                    }
                }
            }

            foreach (int id in volunteersToUpdate)
                Observers.NotifyItemUpdated(id);
        }

        private static void ChooseCallTreatHelp(int volunteerId, int callId)
        {
            // Retrieve the call data based on the callId
            DO.Call? doCall;
            lock (AdminManager.BlMutex)  //stage 7
                doCall = s_dal.Call.Read(c => c.ID == callId);

            // Retrieve all assignments related to the call
            IEnumerable<Assignment>? assignmentsForCall;
            lock (AdminManager.BlMutex)  //stage 7
                assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == callId);

            // Check if the call has already been treated
            BO.Status callStatus = CallsManager.GetCallStatus(doCall);
            if (callStatus == BO.Status.Close)
            {
                throw new InvalidOperationException("The call has already been treated.");
            }

            // Check if there is an open assignment for the call
            var openAssignment = assignmentsForCall.FirstOrDefault(a => a.startTreatment != default(DateTime) && a.finishT == null);
            if (openAssignment != null)
            {
                throw new InvalidOperationException("The call is already being treated.");
            }

            // Check if the call has expired
            if (callStatus == BO.Status.Expired)
            {
                throw new InvalidOperationException("The call has expired.");
            }


            // Once all checks pass, create a new assignment
            Assignment? newAssignment;
            lock (AdminManager.BlMutex)  //stage 7
            {
                newAssignment = new DO.Assignment
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    startTreatment = s_dal.Config.Clock,  // Set the entry time for the treatment
                    finishTreatment = null,  // Treatment is not finished yet
                    finishT = null  // Treatment type is not specified yet
                };
            }

            // Attempt to add the new assignment to the data layer
            lock (AdminManager.BlMutex)  //stage 7
                s_dal.Assignment.Create(newAssignment);
            Observers.NotifyItemUpdated(volunteerId);
            Observers.NotifyListUpdated();
            CallsManager.Observers.NotifyItemUpdated(newAssignment.CallId);  //stage 5
            CallsManager.Observers.NotifyListUpdated();  //stage 5
        }

        private static void FinishTreatHelp(int volunteerId, int assignmentId)
        {
            DO.Assignment assignment;
            try
            {
                lock (AdminManager.BlMutex)
                    assignment = s_dal.Assignment.Read(a => a.ID == assignmentId)
                        ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.");
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
            }

            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.VolunteerCantUpadeOtherVolunteerException($"Volunteer with ID {volunteerId} is not authorized to finish this assignment.");
            }

            if (assignment.finishTreatment != null || assignment.finishT != null)
            {
                throw new BO.AssignmentAlreadyClosedException($"Assignment with ID {assignmentId} is already closed.");
            }

            assignment = assignment with
            {
                finishTreatment = s_dal.Config.Clock,
                finishT = DO.FinishType.Treated
            };

            try
            {
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Update(assignment);
                VolunteersManager.Observers.NotifyItemUpdated(volunteerId);
                VolunteersManager.Observers.NotifyListUpdated();
                CallsManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
                CallsManager.Observers.NotifyListUpdated();  //stage 5
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"An error occurred while updating the assignment.", ex);
            }
        }
        private static void cancelTreatHelp(int volunteerId, int assignmentId)
        {
            DO.Assignment assignment;
            DO.Call call;

            try
            {
                lock (AdminManager.BlMutex)
                    assignment = s_dal.Assignment.Read(a => a.ID == assignmentId)
                    ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.");
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
            }

            try
            {
                lock (AdminManager.BlMutex)
                    call = s_dal.Call.Read(c => c.ID == assignment.CallId)
                    ?? throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} does not exist.");
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} does not exist.", ex);
            }

            if (call.maxTime.HasValue && s_dal.Config.Clock > call.maxTime.Value)
            {
                throw new BO.CantUpdatevolunteer($"Call with ID {assignment.CallId} is expired and cannot be canceled.");
            }
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
            {
                volunteer = s_dal.Volunteer.Read(v => v.ID == volunteerId);
            }
            if (volunteer?.role.Equals(DO.RoleType.Manager) == true &&
                volunteer?.ID != assignment.VolunteerId)
            {
                throw new BO.VolunteerCantUpadeOtherVolunteerException($"Volunteer with ID {volunteerId} is not authorized to cancel this assignment.");
            }

            if (assignment.finishTreatment != null || assignment.finishT != null)
            {
                throw new BO.CantUpdatevolunteer($"Assignment with ID {assignmentId} is already closed or cancelled.");
            }

            assignment = assignment with
            {
                finishTreatment = s_dal.Config.Clock,
                finishT = (assignment.VolunteerId == volunteerId) ? DO.FinishType.SelfCancel : DO.FinishType.ManagerCancel
            };

            try
            {
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Update(assignment);
                VolunteersManager.Observers.NotifyItemUpdated(volunteerId);
                VolunteersManager.Observers.NotifyListUpdated();
                CallsManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
                CallsManager.Observers.NotifyListUpdated();  //stage 5
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"An error occurred while updating the assignment.", ex);
            }
        }
        private static IEnumerable<OpenCallInList> ReadOpenCallsVolunteerHelp(int id)
        {
            IEnumerable<BO.OpenCallInList> openCallInLists;

            lock (AdminManager.BlMutex)
            {
                // קריאת כל הקריאות ממקור הנתונים
                IEnumerable<DO.Call> previousCalls = s_dal.Call.ReadAll(null);

                // קריאת פרטי המתנדב לפי המזהה
                DO.Volunteer volunteerData = s_dal.Volunteer.Read(v => v.ID == id)
                    ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.");

                // יצירת רשימת הקריאות הפתוחות המתאימות
                List<BO.OpenCallInList> calls = new List<BO.OpenCallInList>();
                calls.AddRange(
                    from item in previousCalls
                    let dataCall = ReadCallHelp(item.ID)
                    where dataCall.statusC == BO.Status.Open || dataCall.statusC == BO.Status.OpenInRisk
                    let openCall = CallsManager.ConvertDOCallToBOOpenCallInList(item, id)
                    where volunteerData.maxDistance == null || volunteerData.maxDistance >= openCall.distance
                    select openCall
                );

                openCallInLists = calls;
            }

            return openCallInLists;
        }

        private static BO.Call ReadCallHelp(int id)
        {
            DO.Call? doCall;
            IEnumerable<DO.Assignment> assignmentsForCall;
            lock (AdminManager.BlMutex)
            {
                doCall = s_dal.Call.Read(c => c.ID == id);
                assignmentsForCall = s_dal.Assignment.ReadAll(a => a.CallId == id);

            }


            if (doCall == null)
                throw new BO.BlDoesNotExistException($"Call with ID {id} does not exist in the database.");


            var boCall = CallsManager.ConvertDOCallWithAssignments(doCall, assignmentsForCall);

            return boCall;

        }
    }
}