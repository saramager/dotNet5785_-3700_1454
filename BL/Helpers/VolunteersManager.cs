using BlApi;
using BlImplementation;
using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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
        internal static BO.CallInProgress? GetCallIn(DO.Volunteer doVolunteer)
        {

            var call = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.ID).ToList();
            DO.Assignment? assignmentTreat = call.Find(ass => ass.finishTreatment == null);
            
            if (assignmentTreat != null)
            {
                DO.Call? callTreat = s_dal.Call.Read(c => c.ID == assignmentTreat.CallId);
              
                if (callTreat != null)
                   {
                    double latitude = doVolunteer.Latitude ?? callTreat.latitude;
                    double longitude = doVolunteer.Longitude ?? callTreat.longitude;
                    IAdmin adminImpl = new AdminImplementation(); // יצירת מופע של המחלקה המיישמת את IAdmin
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
                        CallDistance = Tools.CalculateDistance(callTreat.latitude, callTreat.longitude, latitude, longitude),
                        statusT = (callTreat.maxTime - ClockManager.Now <= adminImpl.GetRiskRange() ? BO.Status.InRiskTreat : BO.Status.InTreat),
                    };}
            }
            return null;
        }

        internal static bool checkeVolunteerFormat(BO.Volunteer volunteer)
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
            if (string.IsNullOrEmpty(volunteer.phone) || volunteer.phone.Length < 8 || volunteer.phone.Length > 9 || !(volunteer.phone.All(char.IsDigit)))
                return false;
            if (volunteer.maxDistance < 0)
                return false;



            
            return true;


        }
        internal static bool checkeVolunteerlogic(BO.Volunteer volunteer)
        {
            if (!(IsValidId(volunteer.Id)))
                return false;
            if (volunteer!= null && !IsStrongPassword(volunteer.password!) )
                return false;
            // add adress
            return true;
        }
        internal static bool IsValidId(long id)
        {
            /// <summary>
            /// Validates an Israeli 9-digit ID using the Luhn algorithm.
            /// </summary>
            /// <param name="id">The ID number to validate.</param>
            /// <returns>True if the ID is valid, false otherwise.</returns>
            /// <remarks>
            /// This code was written with the assistance of ChatGPT, a language model developed by OpenAI.
            /// </remarks>

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

        internal static DO.Volunteer convertFormBOVolunteerToDo(BO.Volunteer BoVolunteer)
        {
           
            if (BoVolunteer.currentAddress !=null)
            {
                try
                {
                   double[] cordintes = Tools.GetGeolocationCoordinates(BoVolunteer.currentAddress);
                    BoVolunteer.Latitude = cordintes[0];
                    BoVolunteer.Longitude = cordintes[1];
                }
                catch (Exception e) { }

            }
            else
            {
                BoVolunteer.Latitude = null;
                BoVolunteer.Longitude = null;
            }
            DO.Volunteer doVl = new(
                 ID: BoVolunteer.Id,
                 fullName: BoVolunteer.fullName,
                  phone: BoVolunteer.phone,
                 email: BoVolunteer.email,
                 active: BoVolunteer.active,
                 role: (DO.RoleType)BoVolunteer.role,
                distanceType: (DO.Distance)BoVolunteer.distanceType,
               password: BoVolunteer.password != null ? Encrypt(BoVolunteer.password): null,
               currentAddress: BoVolunteer.currentAddress,
              Latitude: BoVolunteer.Latitude,
              Longitude: BoVolunteer.Longitude,
             maxDistance: BoVolunteer.maxDistance

                        );
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
        
    
    }

}




