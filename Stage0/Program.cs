using System.Diagnostics;
using System.Net;
using System.Text.Json;

partial class Program
{
    private static void Main(string[] args)
    {
        Welcome1454();
        Console.ReadKey();
        Welcome3700();

    }

    //    Console.Write("הכנס מחרוזת: ");
    //    string adress = Console.ReadLine();
    //    var con = GetGeolocationCoordinates(adress);
    //    Console.WriteLine($"lat :{con[0]} lond :{con[1]}");

        //}
        /// <summary>
        /// Retrieves geolocation coordinates (latitude and longitude) for a given address using an external geocoding API.
        /// </summary>
        /// <param name="address">Address to search for</param>
        /// <returns>Array containing latitude and longitude</returns>
        /// <remarks>
        /// Written with the help of ChatGPT from OpenAI (https://openai.com).
        /// </remarks>
    internal static double[] GetGeolocationCoordinates(string address)
    {
        // Check if the address is valid
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        string apiKey = "67575890e42cc578964142uob149f45";  // החלף במפתח האמיתי שלך
        string requestUrl = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
        webRequest.Method = "GET";

        try
        {
            // Send request and get response
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Request failed with status: {webResponse.StatusCode}");
                }

                using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string jsonResponse = responseReader.ReadToEnd();
                    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var locationData = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, jsonOptions);

                    if (locationData == null || locationData.Length == 0)
                    {
                        throw new Exception("No geolocation data found for the given address.");
                    }

                    // Return latitude and longitude
                    return new double[] { double.Parse(locationData[0].Lat), double.Parse(locationData[0].Lon) };
                }
            }
        }
        catch (WebException ex)
        {
            throw new Exception("Error sending web request: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
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
    static partial void Welcome3700();
    private static void Welcome1454()
    {
        Console.Write("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console application", name);
    }
}