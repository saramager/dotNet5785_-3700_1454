using BlApi;
using BlImplementation;
using BO;
using DalApi;
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
                CallId = call == null ? null : call.CallId,
                callT = callD == null ? CallType.None : (BO.CallType)callD.callT
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
        /// get volunteer and return Call in prgers if there is one 
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
                        if (callTreat.latitude!=null && callTreat.longitude!= null)
                         
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
            null,null,
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
        /// <summary>
        /// Simulates volunteer activity by randomly selecting calls for treatment.
        /// </summary>
        /// <returns></returns>
        internal static List<BO.Volunteer> GetActiveVolunteers()
        {
            List<DO.Volunteer> doVolunteers;
            lock (s_dal)
            {
                doVolunteers = s_dal.Volunteer.ReadAll(v => v.active).ToList();// Read all active volunteers from the DAL
            }

            return doVolunteers.Select(convertDOToBOVolunteer).ToList();// Convert the volunteers to BO and return the list
        }

        private static readonly object _lock = new object();// Lock object for synchronization
        private static readonly Random _random = new Random();// Random number generator for the simulation
        /// <summary>
        /// Calculates the time required for a volunteer to treat a call based on the distance and time.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <param name="callInProgress"></param>
        /// <returns></returns>
        private static TimeSpan CalculateRequiredTime(Volunteer volunteer, CallInProgress callInProgress)
        {
            // Calculate the time needed for the treatment based on the distance and time
            return TimeSpan.FromMinutes(_random.Next(5, 15)); // Random time between 5 and 15 minutes
        }
      
        

        /// <summary>
        /// Simulates volunteer activity by randomly selecting calls for treatment.
        /// </summary>
        internal static void SimulateVolunteerActivity()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    List<BO.Volunteer> activeVolunteers;
                    lock (AdminManager.BlMutex)
                    {
                        activeVolunteers = GetActiveVolunteers(); // Receiving a list of active volunteers
                    }

                    // Synchronized on all volunteers
                    foreach (var volunteer in activeVolunteers)
                    {
                        if (volunteer.callProgress == null) // Volunteer without reading in treatment
                        { 
                            if (_random.NextDouble() < 0.2) // 20% probability to select a call for treatment
                            {
                                lock (AdminManager.BlMutex)
                                {
                                    var allCalls = s_dal.Call.ReadAll().ToList(); // Read all calls from the DAL
                                    var possibleCalls = allCalls
                                        .Where(c => CallsManager.GetCallStatus(c) == BO.Status.Open && c.latitude != null && c.longitude != null) // filter by open calls with coordinates
                                        .ToList();

                                    if (possibleCalls.Any())
                                    {
                                        // Random choice of reading
                                        var call = possibleCalls[_random.Next(possibleCalls.Count)];
                                        volunteer.callProgress = new BO.CallInProgress
                                        {
                                            ID = call.ID,
                                            CallId = call.ID,
                                            callT = (BO.CallType)call.callT,
                                            verbalDescription = call.verbalDescription,
                                            address = call.address,
                                            openTime = call.openTime,
                                            maxTime = call.maxTime,
                                            startTreatment = DateTime.Now,
                                            CallDistance = Tools.CalculateDistance(call.latitude, call.longitude, volunteer.Latitude.Value, volunteer.Longitude.Value, volunteer.distanceType),
                                            statusT = BO.Status.InTreat
                                        };

                                        // Adding a new task to the volunteer
                                        s_dal.Assignment.Create(new DO.Assignment
                                        {
                                            VolunteerId = volunteer.Id,
                                            CallId = call.ID,
                                            startTreatment = DateTime.Now
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Volunteer with reading in therapy
                            var timeSinceStart = DateTime.Now - volunteer.callProgress.startTreatment;
                            var requiredTime = CalculateRequiredTime(volunteer, volunteer.callProgress); // Calculate the time needed

                            if (timeSinceStart >= requiredTime)
                            {
                                // if enough time has passed
                                lock (AdminManager.BlMutex)
                                {
                                    // Update call as handling is finished
                                    s_dal.Assignment.Update(new DO.Assignment
                                    {
                                        ID = volunteer.callProgress.ID,
                                        VolunteerId = volunteer.Id,
                                        CallId = volunteer.callProgress.CallId,
                                        startTreatment = volunteer.callProgress.startTreatment,
                                        finishT = DO.FinishType.Treated
                                    });

                                    volunteer.callProgress = null; // End of treatment
                                }
                            }
                            else if (_random.NextDouble() < 0.1) // 10% probability of canceling treatment
                            {
                                lock (AdminManager.BlMutex)
                                {
                                    // Update call as canceled by the volunteer
                                    s_dal.Assignment.Update(new DO.Assignment
                                    {
                                        ID = volunteer.callProgress.ID,
                                        VolunteerId = volunteer.Id,
                                        CallId = volunteer.callProgress.CallId,
                                        startTreatment = volunteer.callProgress.startTreatment,
                                        finishT = DO.FinishType.SelfCancel
                                    });

                                    volunteer.callProgress = null; // Cancel handling
                                }
                            }
                        }
                    }

                    await Task.Delay(1000); // Wait a second before repeating the simulation
                }
            });
        }
    }
}





