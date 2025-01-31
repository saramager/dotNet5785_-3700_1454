﻿
using BO;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace Helpers
{
    internal static class Tools
    {
        static string openRouteServiceApiKey = "5b3ce3597851110001cf62482999abdea2bf4cfd998ad34761d90c08";
        static string secondAPI = "5b3ce3597851110001cf6248fc840214514d46919ed8d8220701ecb5";
        static string apiKey = "qNm0LxJfsk_2wh7AVzpRLr4ud3CKITJqOufRkJhpsZs";
        static int request = 0;


        // The generic method works for any object, returning a string of its properties
        public static string ToStringProperty<T>(this T t)
        {
            string str = "";
            if (t != null)
            {
                // Using reflection to get properties of the object
                foreach (PropertyInfo item in t.GetType().GetProperties())
                {
                    var value = item.GetValue(t, null);
                    if (value != null)
                    {
                        str += "\n" + item.Name + ": " + value;
                    }
                }
            }
            return str;
        }
        /// <summary>
        /// Retrieves geolocation coordinates (latitude and longitude) for a given address using an external geocoding API.
        /// </summary>
        /// <param name="address">Address to search for</param>
        /// <returns>Array containing latitude and longitude</returns>
        /// <remarks>
        /// Written with the help of ChatGPT from OpenAI (https://openai.com).
        /// </remarks>
        internal async static Task<double[]> GetGeolocationCoordinatesAsync(string address)
        {
            Thread.Sleep(200);
            // Check if the address is valid
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty or null.", nameof(address));
            }

            string apiKey = "67609238e7135923908907oxh46a576";
            string requestUrl = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            webRequest.Method = "GET";

            try
            {
                // Send request and get response
                using (HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync())
                {
                    if (webResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Request failed with status: {webResponse.StatusCode}");
                    }

                    using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string jsonResponse = responseReader.ReadToEnd();
                        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var locationData = System.Text.Json.JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, jsonOptions);


                        if (locationData == null || locationData.Length == 0)
                        {
                            throw new AdressDoesNotExistException("No geolocation data found for the given address.");
                        }
                        if (locationData.Length > 1)

                            throw new AdressDoesNotExistException("The address is not speasific.");

                        // Return latitude and longitude
                        return new double[] { double.Parse(locationData[0].Lat), double.Parse(locationData[0].Lon) };
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception("Error sending web request: " + ex.Message);
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("An error occurred: " + ex.Message);
            //}
        }

        /// <summary>
        /// Represents a location result returned by the geocoding service.
        /// </summary>
        /// <remarks>
        /// This class was created with the help of ChatGPT from OpenAI (https://openai.com).
        /// </remarks>
        public class LocationResult
        {
            /// <summary>
            /// Latitude of the location.
            /// </summary>
            public string Lat { get; set; }

            /// <summary>
            /// Longitude of the location.
            /// </summary>
            public string Lon { get; set; }

            // Add additional properties if needed (e.g., address, city, country)
        }
        /// <summary>
        /// Calculates the distance between two geographical points using Haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>The distance between the two points in kilometers.</returns>
        /// <remarks>
        /// Code by ChatGPT (OpenAI).
        /// </remarks>
        /// 

        public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2, BO.Distance distanceType)
        {

            double distance = 0; // נכון עבור nullable double

            if (lat1 != null && lon1 != null && lat2 != null && lon2 != null)
            {
                switch (distanceType)
                {
                    case BO.Distance.AirDistance:
                        const double R = 6371; // Radius of the Earth in kilometers

                        double lat1Rad = ToRadians(lat1 ?? 0);
                        double lon1Rad = ToRadians(lon1 ?? 0);
                        double lat2Rad = ToRadians(lat2 ?? 0);
                        double lon2Rad = ToRadians(lon2 ?? 0);

                        double dLat = lat2Rad - lat1Rad;
                        double dLon = lon2Rad - lon1Rad;

                        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                        distance = R * c; // Distance in kilometers
                        break;
                    case BO.Distance.walkingDistance:
                        //try
                        //{
                        //    distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "foot-walking")/*.Result*/;
                        //}
                        //catch (Exception ex)
                        //{

                        //    //Thread.Sleep(new TimeSpan(0, 1, 0));
                        if (request < 40)
                        {
                            distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "foot-walking", openRouteServiceApiKey);
                            request++;
                        }
                        else if (request < 80)
                        {
                            distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "foot-walking", secondAPI);
                            request++;
                        }
                        else if (request < 100)
                        {
                            
                                distance = GetRouteDistanceSync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "pedestrian");
                                request++;
                                if (request == 100)
                                    request = 0;
                           
                        }
                        break;

                    case BO.Distance.DrivingDistance:
                        //try
                        //{
                        //    distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "driving-car")/*.Result*/;
                        //}
                        //catch (Exception ex)
                        //{
                        //    //Thread.Sleep(new TimeSpan(0, 1, 0));
                        //    //distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "driving-car");
                        //    throw new Exception(ex.Message);
                        //}

                        if (request < 40)
                        {
                            distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "driving-car", openRouteServiceApiKey);
                            request++;
                        }
                        else if (request < 80)
                        {
                            distance = GetDistanceNotAsync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "driving-car", secondAPI);
                            request++;
                        }
                        else if (request < 60)
                        {
                            
                                distance = GetRouteDistanceSync(lat1 ?? 0, lon1 ?? 0, lat2 ?? 0, lon2 ?? 0, "car");
                                request++;
                                if (request == 60)
                                    request = 0;
                            
                           
                        }
                        break;

                }
            }
            return distance;

        }



        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The degree value to convert.</param>
        /// <returns>The corresponding value in radians.</returns>
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }


        /// <summary>
        /// Sends a request to the OpenRouteService API and retrieves the distance 
        /// between two points based on the selected transportation mode.
        /// </summary>
        /// <param name="lat1">Latitude of the starting point.</param>
        /// <param name="lon1">Longitude of the starting point.</param>
        /// <param name="lat2">Latitude of the destination point.</param>
        /// <param name="lon2">Longitude of the destination point.</param>
        /// <param name="profile">The transportation mode (e.g., "driving-car" or "foot-walking").</param>
        /// <returns>The distance in kilometers between the two points, or -1 if an error occurs.</returns>
        /// <exception cref="Exception">Thrown when there is an issue with the API request or response.</exception>
        //public static async Task<double> GetDistanceAsync(double lat1, double lon1, double lat2, double lon2, string profile)
        //{
        //    Thread.Sleep(200);

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            string url = $"https://api.openrouteservice.org/v2/directions/{profile}/json";
        //            var body = new
        //            {
        //                coordinates = new[]
        //                {
        //                    new[] { lon1, lat1 },
        //                    new[] { lon2, lat2 }
        //                }
        //            };

        //            string jsonBody = JsonConvert.SerializeObject(body);
        //            client.DefaultRequestHeaders.Add("Authorization", openRouteServiceApiKey);

        //            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        //            HttpResponseMessage response = await client.PostAsync(url, content);

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                throw new Exception($"Error: {response.StatusCode}");
        //            }

        //            string responseBody = response.Content.ReadAsStringAsync().Result;
        //            dynamic? responseData = JsonConvert.DeserializeObject(responseBody);
        //            if (responseData == null)
        //            {
        //                throw new Exception("Deserialization returned null.");
        //            }

        //            if (responseData.routes != null && responseData.routes.Count > 0)
        //            {
        //                var route = responseData.routes[0];
        //                return route.summary.distance / 1000.0;
        //            }
        //            else
        //            {
        //                throw new Exception("No route found.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("An error occurred: " + ex.Message);
        //        }
        //    }
        //}

        public static double GetDistanceNotAsync1(double lat1, double lon1, double lat2, double lon2, string profile)
        {
            Thread.Sleep(200);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://api.openrouteservice.org/v2/directions/{profile}/json";
                    var body = new
                    {
                        coordinates = new[]
                        {
                    new[] { lon1, lat1 },
                    new[] { lon2, lat2 }
                }
                    };

                    string jsonBody = JsonConvert.SerializeObject(body);
                    client.DefaultRequestHeaders.Add("Authorization", openRouteServiceApiKey);

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error: {response.StatusCode}");
                    }

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    dynamic? responseData = JsonConvert.DeserializeObject(responseBody);
                    if (responseData == null)
                    {
                        throw new Exception("Deserialization returned null.");
                    }

                    if (responseData.routes != null && responseData.routes.Count > 0)
                    {
                        var route = responseData.routes[0];
                        return route.summary.distance / 1000.0;
                    }
                    else
                    {
                        throw new Exception("No route found.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred: " + ex.Message);
                }
            }
        }





        public static double GetDistanceNotAsync(double lat1, double lon1, double lat2, double lon2, string profile,string ApiKeys)
        {
            try
            {
                string url = $"https://api.openrouteservice.org/v2/directions/{profile}/json";
                var body = new
                {
                    coordinates = new[] {
                    new[] { lon1, lat1 },
                    new[] { lon2, lat2 }
                }
                };

                string jsonBody = JsonConvert.SerializeObject(body);

                // יצירת WebRequest
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", ApiKeys);

                // כתיבה ל-stream של הבקשה
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonBody);
                }

                // קבלת התשובה באופן סינכרוני
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string responseBody = reader.ReadToEnd();
                    dynamic responseData = JsonConvert.DeserializeObject(responseBody);

                    if (responseData?.routes != null && responseData.routes.Count > 0)
                    {
                        var route = responseData.routes[0];
                        return route.summary.distance / 1000.0; // המר את המרחק מקילומטרים
                    }
                    else
                    {
                        throw new Exception("No route found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw new Exception("An error occurred: " + ex.Message);
            }
        }


        public static double GetRouteDistanceSync(double lat1, double lon1, double lat2, double lon2, string transportMode)
        {
            try
            {
                // בניית URL לבקשה
                string url = $"https://router.hereapi.com/v8/routes?apikey={apiKey}&transportMode={transportMode}&origin={lat1},{lon1}&destination={lat2},{lon2}&return=summary";

                // יצירת WebRequest סינכרונית
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                // קבלת התשובה באופן סינכרוני
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string responseBody = reader.ReadToEnd();
                    dynamic responseData = JsonConvert.DeserializeObject(responseBody);

                    // בדיקה אם יש תוצאה תקינה
                    if (responseData?.routes != null && responseData.routes.Count > 0)
                    {
                        var route = responseData.routes[0];
                        return route.sections[0].summary.length / 1000.0; // המרחק בקילומטרים
                    }
                    else
                    {
                        throw new Exception("No route found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw new Exception("Error fetching route data: " + ex.Message);
            }
        }
    }
}



