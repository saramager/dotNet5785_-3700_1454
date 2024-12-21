namespace BlTest;
using BlApi;
using BO;
using System;
using System.Linq.Expressions;

public enum OPTION
{
    EXIT,
    ADMIN_MENUE,
    VOLUNTEER_MENUE,
    CALL_MENUE,
    //SHOW_ALL_DB,
}
public enum IAdmin
{
    EXIT,
    GET_CLOCK,
    FORWARD_CLOCK,
    GET_MAX_RANGE,
    DEFENIATION,
    RESET,
    INITIALIZATION,
}
public enum IVolunteer
{
    EXIT,
    ENTER_SYSTEM,
    GET_VOlUNTEERLIST,
    READ,
    UPDATE,
    DELETE,
    CREATE,
}
public enum ICall
{
    EXIT,
    COUNT_CALL,
    GET_CALLINLIST,
    READ,
    UPDATE,
    DELETE,
    CREATE,
    GET_CLOSED_CALL,
    GET_OPEN_CALL,
    CLOSE_TREAT,
    CANCEL_TREAT,
    CHOSE_TOR_TREAT,
}

internal class Program
{


    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static void Main(string[] args)
    {
        try //If there are any exceptions
        {
            OPTION option = showMainMenu();
            while (OPTION.EXIT != option)  //As long as you haven't chosen an exit
            {
                switch (option)
                {
                    case OPTION.ADMIN_MENUE:
                        handleAdminOptions();
                        break;
                    case OPTION.VOLUNTEER_MENUE:
                        handleVolunteerOptions();
                        break;
                    case OPTION.CALL_MENUE:
                        handleCallOptions();
                        break;
                }
                option = showMainMenu();
            }
        }
        catch (Exception ex)   //If any anomaly is detected
        {
            Console.WriteLine(ex);
        }
    }

    private static OPTION showMainMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@"
OPTION Options:
0 - Exit
1 - Admin
2 - Volunteer
3 - Call ");

        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (OPTION)choice;
    }
    private static IAdmin showAdminMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
Config Options:
0 - Exit
1-  Get clock
2 - Forward Clock 
3 - Get RiskRange
4 - Set RiskRange
5 - Reset
6 - initialization");

        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (IAdmin)choice;
    }

    private static void handleAdminOptions()
    {
        try
        {
            switch (showAdminMenu())
            {
                case IAdmin.GET_CLOCK:
                    Console.WriteLine(s_bl.Admin.GetClock());
                    break;
                case IAdmin.FORWARD_CLOCK:
                    // Prompt the user to enter a time unit (e.g., MINUTE, HOUR, DAY, etc.)
                    Console.WriteLine("Enter unit time:  Minute, Hour, Day, Month, Year");
                    string inputClock = Console.ReadLine() ?? throw new Exception("did not get value");// Convert input to uppercase for consistency
                    // Try to parse the user input into the BO.TimeUnit enum
                    if (!Enum.TryParse(typeof(BO.TimeUnit), inputClock, out object unitObj) || unitObj is not BO.TimeUnit unit)
                    {
                        // If parsing fails or the input is not valid, throw an exception
                        throw new BO.BlWrongInputException("Wrong input. Please enter a valid time unit.");
                    }

                    // Call the ForwardClock method with the parsed time unit
                    s_bl.Admin.AddToClock(unit);
                    break;
                case IAdmin.GET_MAX_RANGE:
                    Console.WriteLine(s_bl.Admin.GetRiskRange());
                    break;
                case IAdmin.DEFENIATION:
                    Console.WriteLine("Enter time span in the format (days:hours:minutes:seconds):");
                    string inputTimeSpan = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    if (!TimeSpan.TryParse(inputTimeSpan, out TimeSpan time))
                    {
                        throw new BO.BlWrongInputException("Invalid input. Please enter a valid time span.");
                    }

                    s_bl.Admin.SetRiskRange(time);
                    break;
                case IAdmin.RESET:
                    s_bl.Admin.RestartDB();
                    break;
                case IAdmin.INITIALIZATION:
                    s_bl.Admin.UpdateDB();
                    break;
                case IAdmin.EXIT:
                    break;
                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }
        catch (BO.BlWrongInputException ex)
        {

            Console.WriteLine($"Error: {ex.Message}");
        }

    }
    private static IVolunteer showVolunteerMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@"
OPTION Options:
0 - Exit
1 - EnterSystem
2 - Get volunteerInlist
3 - Read
4 - update
5 - Delete
6 - Create");

        }
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (IVolunteer)choice;
    }
    private static void handleVolunteerOptions()
    {
        try
        {
            switch (showVolunteerMenu())
            {

                case IVolunteer.ENTER_SYSTEM:

                    Console.WriteLine("Please enter your username:");
                    if (!int.TryParse(Console.ReadLine(), out int username))
                    {
                        throw new BO.BlWrongInputException("Invalid username. It must be a numeric value.");
                    }

                    Console.WriteLine("Please enter your password:");
                    string password = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    // Call the EnterSystem method to verify credentials and get the user's role
                    BO.RoleType role = s_bl.Volunteer.EnterToSystem(username, password);
                    Console.WriteLine($"Welcome to the system! Your role is: {role}");
                    break;
                case IVolunteer.GET_VOlUNTEERLIST:

                    Console.WriteLine("Would you like to filter by active status? (true/false/null):");
                    string activeInput = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    bool? activeFilter = activeInput.ToLower() == "true" ? true
                                        : activeInput.ToLower() == "false" ? false
                                        : (bool?)null;

                    Console.WriteLine("Please select a sort option (ID , fullName ,active , numCallsHandled ,numCallsCancelled ,numCallsExpired,CallId , callT):");
                    string sortByInput = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    BO.FiledOfVolunteerInList? sortBy = Enum.TryParse<BO.FiledOfVolunteerInList>(sortByInput, true, out var sortOption) ? sortOption : null;


                    IEnumerable<BO.VolunteerInList> volunteerList = s_bl.Volunteer.GetVolunteerInList(activeFilter, sortBy);


                    Console.WriteLine("Volunteer List:");
                    foreach (var Volunteer in volunteerList)
                    {
                        Console.WriteLine(Volunteer.ToString());
                    }
                    break;
                case IVolunteer.READ:
                    Console.WriteLine("Please enter the ID of the volunteer:");
                    string idInput = Console.ReadLine() ?? throw new FormatException("Wrong input");  // לוקחים את הקלט מהמשתמש

                    if (!int.TryParse(idInput, out int id))  // מנסים להמיר את הקלט למספר
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idInput} format");  // זורקים חריגה אם המזהה לא תקני
                    }

                    BO.Volunteer volunteer = s_bl.Volunteer.ReadVolunteer(id);
                    Console.WriteLine("Volunteer Details:");
                    Console.WriteLine(volunteer);
                    break;
                case IVolunteer.UPDATE:

                    Console.WriteLine("Please enter the ID of the volunteer you want to update:");
                    int idToUpdate;
                    if (!int.TryParse(Console.ReadLine(), out idToUpdate))
                    {
                        throw new BO.BlWrongInputException("Invalid ID. Please enter a valid number.");
                    }
                    Console.WriteLine("Please enter the your ID");
                    int idPersson;
                    if (!int.TryParse(Console.ReadLine(), out idPersson))
                    {
                        throw new BO.BlWrongInputException("Invalid ID. Please enter a valid number.");
                    }

                    // Try to retrieve the volunteer by ID


                    Console.WriteLine("Updating details for volunteer:");
                    //  Console.WriteLine(volunteerToUpdate);

                    // Full Name
                    Console.WriteLine("Full Name :");
                    string fullName = Console.ReadLine() ?? throw new FormatException("Wrong input");
                    if (string.IsNullOrEmpty(fullName))
                    {
                        throw new BO.BlWrongInputException("Invalid fullName. Please enter a valid number.");
                    }
                    // Phone Number

                    Console.WriteLine("Phone Number :");
                    string phoneNumber = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    if (string.IsNullOrEmpty(phoneNumber))
                    {
                        throw new BO.BlWrongInputException("Invalid phoneNumber. Please enter a valid number.");
                    }

                    // Email
                    Console.WriteLine("Email:");
                    string email = Console.ReadLine() ?? throw new FormatException("Wrong input");

                    // Distance Type
                    Console.WriteLine("Distance Type (AirDistance,walkingDistance,DrivingDistance) for Arial leav empty:");
                    string distanceTypeInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
                    //if (!string.IsNullOrEmpty(distanceTypeInput))
                    //{
                    BO.Distance distanceTypeUpdate;
                    if (!BO.Distance.TryParse(distanceTypeInput, true, out distanceTypeUpdate))
                    {
                        distanceTypeUpdate = BO.Distance.AirDistance;
                    }

                    //}

                    // Role
                    Console.WriteLine("Role (  Manager, TVolunteer):");
                    //string roleInput = Console.ReadLine();
                    //if (!string.IsNullOrEmpty(roleInput))
                    //{
                    BO.RoleType roleUpdate;
                    if (!Enum.TryParse(Console.ReadLine(), true, out roleUpdate))
                    {
                        throw new BO.BlWrongInputException("Invalid role.");
                    }
                    //}

                    // Active
                    Console.WriteLine("Active (true/false): defult is true");

                    bool activeUP;
                    if (!bool.TryParse(Console.ReadLine(), out activeUP))
                    {
                        activeUP = true;
                    }


                    // Password
                    Console.WriteLine("Password :");
                    string passwordNew = Console.ReadLine() ?? throw new FormatException("Wrong input");


                    // Full Address
                    Console.WriteLine("Full Address:");
                    string fullAddress = Console.ReadLine() ?? throw new FormatException("Wrong input");
                    if (string.IsNullOrEmpty(fullAddress))
                    {
                        throw new BO.BlWrongInputException("the addres not valid");
                    }



                    // Max Reading
                    Console.WriteLine("Max Reading:");

                    int maxReadingUP;
                    if (!int.TryParse(Console.ReadLine(), out maxReadingUP))
                    {
                        throw new BO.BlWrongInputException("Invalid input for Max Reading. Keeping current value.");
                    }

                    BO.Volunteer volunteerToUpdate = new BO.Volunteer
                    {
                        Id = idToUpdate,
                        fullName = fullName,
                        phone = phoneNumber,
                        email = email,
                        distanceType = distanceTypeUpdate,
                        role = roleUpdate,
                        active = activeUP,
                        password = passwordNew,
                        currentAddress = fullAddress,
                        Latitude = 0,
                        Longitude = 0,
                        maxDistance = maxReadingUP,
                        numCallsHandled = 0,
                        numCallsCancelled = 0,
                        numCallsExpired = 0,
                        callProgress = null,
                    };


                    // Update the volunteer in the system

                    s_bl.Volunteer.UpdateVolunteer(idPersson, volunteerToUpdate);
                    break;

                case IVolunteer.DELETE:
                    Console.WriteLine("Volunteer id:");
                    string idInputDelete = Console.ReadLine() ?? throw new FormatException("Wrong input");
                    if (!int.TryParse(idInputDelete, out int idDelete))
                    {
                        throw new BO.BlWrongInputException($"Invalid ID{idInputDelete} format");
                    }
                    s_bl.Volunteer.DeleteVolunteer(idDelete);
                    break;
                case IVolunteer.CREATE:
                    {

                        Console.WriteLine("Please enter volunteer details:");
                        Console.WriteLine("Please enter the ID :");
                        string idCreat = Console.ReadLine() ?? throw new FormatException("Wrong input");  // לוקחים את הקלט מהמשתמש

                        if (!int.TryParse(idCreat, out int idC))  // מנסים להמיר את הקלט למספר
                        {
                            throw new BO.BlWrongInputException($"Invalid ID{idCreat} format");  // זורקים חריגה אם המזהה לא תקני
                        }
                        // Full Name
                        Console.WriteLine("Full Name:");
                        string fullNameUp = Console.ReadLine() ?? throw new FormatException("Wrong input");

                        // Phone Number
                        Console.WriteLine("Phone Number:");
                        string phoneNumberUp = Console.ReadLine() ?? throw new FormatException("Wrong input");

                        // Email
                        Console.WriteLine("Email:");
                        string emailUp = Console.ReadLine() ?? throw new FormatException("Wrong input");

                        // Distance Type
                        Console.WriteLine("Distance Type (AirDistance, walkingDistance, DrivingDistance):");
                        string distanceTypeInputUp = Console.ReadLine() ?? throw new FormatException("Wrong input");
                        BO.Distance distanceType;
                        if (!Enum.TryParse(distanceTypeInputUp, true, out distanceType))
                        {
                            throw new BO.BlWrongInputException("Invalid distance type. Defaulting to Aerial.");

                        }

                        // Role
                        Console.WriteLine("Role ( Manager, TVolunteer):");
                        string roleUp = Console.ReadLine() ?? throw new FormatException("Wrong input");
                        BO.RoleType roleup;
                        if (!Enum.TryParse(roleUp, true, out roleup))
                        {
                            throw new BO.BlWrongInputException("Invalid role. Defaulting to Volunteer.");

                        }

                        // Active
                        Console.WriteLine("Active (true/false):");
                        bool active;
                        if (!bool.TryParse(Console.ReadLine(), out active))
                        {
                            throw new BO.BlWrongInputException("Invalid input for Active. Defaulting to false.");
                        }

                        // Password
                        Console.WriteLine("Password:");
                        string passwordUp = Console.ReadLine() ?? throw new FormatException("Wrong input");

                        // Full Address
                        Console.WriteLine("Full Address:");
                        string fullAddressUp = Console.ReadLine() ?? throw new FormatException("Wrong input");

                        // Max Reading
                        Console.WriteLine("Max Reading:");
                        int maxReading;
                        if (!int.TryParse(Console.ReadLine(), out maxReading))
                        {
                            throw new BO.BlWrongInputException("Invalid input for Max Reading. Defaulting to 0.");

                        }

                        // Create the new Volunteer object
                        BO.Volunteer newVolunteer = new BO.Volunteer
                        {
                            Id = idC,
                            fullName = fullNameUp,
                            phone = phoneNumberUp,
                            email = emailUp,
                            distanceType = distanceType,
                            role = roleup,
                            active = active,
                            password = passwordUp,
                            currentAddress = fullAddressUp,
                            Latitude = 0,
                            Longitude = 0,
                            maxDistance = maxReading,
                            numCallsHandled = 0,
                            numCallsCancelled = 0,
                            numCallsExpired = 0,
                            callProgress = null,

                        };

                        // Call the Create method
                        s_bl.Volunteer.CreateVolunteer(newVolunteer);
                        break;

                    }
            }
        }
        catch (BO.volunteerHandleCallException ex)
        {
            Console.WriteLine($"cant delete because : {ex.Message}");
        }
        catch (BO.BlNullPropertyException ex)
        {
            // Handle the case where the volunteer does not exist
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (BO.BlWrongInputException ex)
        {
            // Handle the case where the password does not match
            Console.WriteLine($"Error: {ex.Message}");
        }
        //catch (BO.BlWrongItemException ex)
        //{
        //    // Handle the case where the password does not match
        //    Console.WriteLine($"Error: {ex.Message}");
        //}
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

    }

    private static ICall showCallMenu()
{
    int choice;
    do
    {
        Console.WriteLine(@"
Call Options:
0 - Exit
1 - CountCall
2 - Get CallInLists
3 - Read
4 - update
5 - Delete
6 - Create
7 - Get ClosedCall
8 - Get OpenCall
9 - CloseTreat
10 - CancelTreat
11- ChoseForTreat");

    }
    while (!int.TryParse(Console.ReadLine(), out choice));
    return (ICall)choice;
}
private static void handleCallOptions()
{
    try
    {
        switch (showCallMenu())
        {
            case ICall.COUNT_CALL:
                //Console.WriteLine(s_bl.Calls.CountCall());
                //   Console.WriteLine(string.Join(", ", s_bl.Calls.CountCall()));
                int[] counts = s_bl.Call.SumOfCalls();

                string[] statusNames = Enum.GetNames(typeof(Status));

                for (int i = 0; i < statusNames.Length; i++)
                {
                    Console.WriteLine($"{statusNames[i]}: {counts[i]}");
                }

                break;
            case ICall.GET_CALLINLIST:
                Console.Write("Enter filter field: ");
                string? filterInput = Console.ReadLine();

                // Try to parse the filter input
                BO.FiledOfCallInList? filter = null;
                if (!string.IsNullOrEmpty(filterInput) && Enum.TryParse(filterInput, out BO.FiledOfCallInList parsedFilter))
                {
                    filter = parsedFilter;
                }
                Console.Write("Enter value for the filter field (or leave blank for no value): ");
                string? filterValueInput = Console.ReadLine();
                object? filterValue = string.IsNullOrEmpty(filterValueInput) ? null : filterValueInput;

                Console.WriteLine("Please choose a sorting field from the following list (or leave blank for default sorting):");
                foreach (var field in Enum.GetNames(typeof(BO.FiledOfCallInList)))
                {
                    Console.WriteLine($"- {field}");
                }
                Console.Write("Enter sorting field: ");
                string? sortInput = Console.ReadLine();

                // Try to parse the sorting input
                BO.FiledOfCallInList? sortBy = null;
                if (!string.IsNullOrEmpty(sortInput) && Enum.TryParse(sortInput, out BO.FiledOfCallInList parsedSort))
                {
                    sortBy = parsedSort;
                }

                // Step 4: Call the method and display the results
                var callInLists = s_bl.Call.GetCallInList(filter, filterValue, sortBy);

                Console.WriteLine("Filtered and sorted call-in list:");
                foreach (var call in callInLists)
                {
                    Console.WriteLine(call); // Assuming BO.CallInList has a meaningful ToString() implementation
                }
                break;
            case ICall.READ:
                Console.WriteLine("Please enter the ID of the call:");
                string idInput = Console.ReadLine();  // לוקחים את הקלט מהמשתמש

                if (!int.TryParse(idInput, out int id))  // מנסים להמיר את הקלט למספר
                {
                    throw new BO.BlWrongInputException($"Invalid ID{idInput} format");  // זורקים חריגה אם המזהה לא תקני
                }
                Console.WriteLine(s_bl.Call.ReadCall(id));
                break;
            case ICall.UPDATE:
                s_bl.Call.UpdateCall(getCall());
                break;
            case ICall.DELETE:
                Console.WriteLine("Please enter the ID of the call:");
                string idDel = Console.ReadLine();  // לוקחים את הקלט מהמשתמש

                if (!int.TryParse(idDel, out int idd))  // מנסים להמיר את הקלט למספר
                {
                    throw new BO.BlWrongInputException($"Invalid ID{idDel} format");  // זורקים חריגה אם המזהה לא תקני
                }
                s_bl.Call.DeleteCall(idd);
                break;
            case ICall.CREATE:
                {
                    //s_bl.Calls.Create(getCall());
                    Console.WriteLine("Enter call type (BabyGift,MomGift,HouseholdHelp,MealPreparation):");
                    string callTypeInput = Console.ReadLine();
                    if (!Enum.TryParse(callTypeInput, true, out BO.CallType callType) || !Enum.IsDefined(typeof(BO.CallType), callType))
                    {
                        throw new BO.BlWrongInputException("Invalid input. Please enter a valid call type (BabyGift,MomGift,HouseholdHelp,MealPreparation):");
                    }

                    Console.WriteLine("Enter description:");
                    string description = Console.ReadLine();

                    Console.WriteLine("Enter full address:");
                    string fullAddress = Console.ReadLine();

                    DateTime timeOpened;
                    Console.WriteLine("Enter time opened (YYYY-MM-DD HH:mm:ss):");
                    if (!DateTime.TryParse(Console.ReadLine(), out timeOpened))
                    {
                        throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time (YYYY-MM-DD HH:mm:ss):");
                    }

                    DateTime? maxTimeToClose = null;
                    Console.WriteLine("Enter max time to close (or leave empty):");
                    string maxTimeInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(maxTimeInput) && !DateTime.TryParse(maxTimeInput, out DateTime parsedMaxTime))
                    {
                        throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time for max time to close.");
                    }

                    //BO.StatusTreat status;
                    //Console.WriteLine("Enter status (e.g., 0 for Pending, 1 for InProgress, 2 for Completed):");
                    //while (!Enum.TryParse(Console.ReadLine(), out status) || !Enum.IsDefined(typeof(BO.StatusTreat), status))
                    //{
                    //    Console.WriteLine("Invalid input. Please enter a valid status (0 for Pending, 1 for InProgress, 2 for Completed):");
                    //}
                    BO.Call callToCreate = new BO.Call
                    {
                        ID = 0,
                        callT = callType,
                        verbalDescription = description,
                        address = fullAddress,
                        latitude = 0,
                        longitude = 0,
                        openTime = timeOpened,
                        maxTime = maxTimeToClose,
                        statusC = 0,
                    };
                    s_bl.Call.CreateCall(callToCreate);

                }
                break;
            case ICall.GET_CLOSED_CALL:
                {
                    // Ask the user for the volunteer's ID
                    Console.Write("Enter volunteer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                    {
                        throw new BO.BlWrongInputException("Invalid ID format.");

                    }

                    // Ask the user for the call type
                    Console.Write("Enter call type or null ( BabyGift, MomGift, HouseholdHelp, MealPreparation or leave blank): ");
                    string callTypeInput = Console.ReadLine();
                    BO.CallType? callType = null;
                    if (!string.IsNullOrEmpty(callTypeInput) && Enum.TryParse(callTypeInput, out BO.CallType parsedCallType))
                    {
                        callType = null;
                    }

                    // Ask the user for the sorting field
                    Console.Write("Enter sorting field  or null(Id, CType, FullAddress, TimeOpen, StartTreat, TimeClose, TypeEndTreat or leave blank): ");
                    string sortByInput = Console.ReadLine();
                    BO.FiledOfClosedCallInList? sortByClose = null;
                    if (!string.IsNullOrEmpty(sortByInput) && Enum.TryParse(sortByInput, out BO.FiledOfClosedCallInList parsedSortBy))
                    {
                        sortBy = null;
                    }

                    // Call the method to get the filtered and sorted closed calls
                    var closedCalls = s_bl.Call.ReadCloseCallsVolunteer(volunteerId, callType, sortByClose);

                    // Display the result
                    Console.WriteLine("Closed Calls:");
                    foreach (var call in closedCalls)
                    {
                        Console.WriteLine(call);
                    }
                    break;
                }

            case ICall.GET_OPEN_CALL:
                {

                    // Ask the user for the volunteer's ID
                    Console.Write("Enter volunteer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                    {
                        Console.WriteLine("Invalid ID format.");
                        break;
                    }

                    //// Ask the user for the volunteer's ID
                    //Console.Write("Enter volunteer ID: ");
                    //if (!int.TryParse(Console.ReadLine(), out int volunteerID))
                    //{
                    //    throw new BO.BlWrongInputException("Invalid ID format.");

                    //}

                    // Ask the user for the call type
                    Console.Write("Enter call type or null ( BabyGift,  MomGift, HouseholdHelp, MealPreparation or leave blank): ");
                    string callTypeInput = Console.ReadLine();
                    BO.CallType? callType = null;
                    if (!string.IsNullOrEmpty(callTypeInput) && Enum.TryParse(callTypeInput, out BO.CallType parsedCallType))
                    {
                        callType = null;
                    }

                    // Ask the user for the sorting field
                    Console.Write("Enter sorting field  or null(Id, CType, FullAddress, TimeOpen, StartTreat, TimeClose, TypeEndTreat or leave blank): ");
                    string sortByInput = Console.ReadLine();
                        BO.FiledOfOpenCallInList? sortByOpen = null;
                        if (!string.IsNullOrEmpty(sortByInput) && Enum.TryParse(sortByInput, out BO.FiledOfOpenCallInList parsedSortBy))
                        {
                            sortByOpen = parsedSortBy;
                        }

                        var closedCalls = s_bl.Call.ReadOpenCallsVolunteer(volunteerId, callType, sortByOpen);

                        // Display the result
                        Console.WriteLine("Open Calls:");
                    foreach (var call in closedCalls)
                    {
                        Console.WriteLine(call);
                    }
                    break;
                }
            case ICall.CLOSE_TREAT:
                {
                    // Requesting the user to enter the values
                    Console.WriteLine("Enter the volunteer ID:");
                    string volInput = Console.ReadLine();
                    Console.WriteLine("Enter the task ID:");
                    string assigInput = Console.ReadLine();

                    // Variables to store the values entered by the user
                    int idVol, idAssig;

                    // Checking if it's possible to parse the input into integers
                    if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                    {
                        // If the parsing succeeded, call the CloseTreat function
                        s_bl.Call.FinishTreat(idVol, idAssig);
                    }
                    else
                    {
                        // If parsing failed, display an error message
                        throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                    }
                    break;
                }
            case ICall.CANCEL_TREAT:
                {
                    // Requesting the user to enter the values
                    Console.WriteLine("Enter the volunteer ID:");
                    string volInput = Console.ReadLine();
                    Console.WriteLine("Enter the task ID:");
                    string assigInput = Console.ReadLine();

                    // Variables to store the values entered by the user
                    int idVol, idAssig;

                    // Checking if it's possible to parse the input into integers
                    if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                    {
                        // If the parsing succeeded, call the CloseTreat function
                        s_bl.Call.cancelTreat(idVol, idAssig);
                    }
                    else
                    {
                        // If parsing failed, display an error message
                        throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                    }
                    break;
                }
            case ICall.CHOSE_TOR_TREAT:
                {
                    // Requesting the user to enter the values
                    Console.WriteLine("Enter the volunteer ID:");
                    string volInput = Console.ReadLine();
                    Console.WriteLine("Enter the task ID:");
                    string assigInput = Console.ReadLine();

                    // Variables to store the values entered by the user
                    int idVol, idAssig;

                    // Checking if it's possible to parse the input into integers
                    if (int.TryParse(volInput, out idVol) && int.TryParse(assigInput, out idAssig))
                    {
                        // If the parsing succeeded, call the CloseTreat function
                        s_bl.Call.ChooseCallTreat(idVol, idAssig);
                    }
                    else
                    {
                        // If parsing failed, display an error message
                        throw new BO.BlWrongInputException("Invalid input. Please ensure the IDs are numbers.");
                    }
                    break;
                }
            default: break;
        }

    }
    catch (BO.BlWrongInputException ex)
    {
        // Handle the case where the password does not match
        Console.WriteLine($"Error: {ex.Message}");
    }
    catch (BO.BlWrongItemException ex)
    {
        // Handle the case where the password does not match
        Console.WriteLine($"Error: {ex.Message}");
    }
    catch (Exception ex)
    {
        // Handle any other unexpected errors
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }

}
private static BO.Call getCall()
{
    int id;
    Console.WriteLine("Enter call ID:");
    if (!int.TryParse(Console.ReadLine(), out id))
    {
        throw new BO.BlWrongInputException("Invalid input. Please enter a valid integer for the ID:");
    }
    Console.WriteLine("Enter call type ( BabyGift,MomGift,HouseholdHelp,MealPreparation):");
    string callTypeInput = Console.ReadLine();
    if (!Enum.TryParse(callTypeInput, true, out BO.CallType callType) || !Enum.IsDefined(typeof(BO.CallType), callType))
    {
        throw new BO.BlWrongInputException("Invalid input. Please enter a valid call type (BabyGift,MomGift,HouseholdHelp,MealPreparation ):" );
    }

    Console.WriteLine("Enter description:");
    string description = Console.ReadLine();

    Console.WriteLine("Enter full address:");
    string fullAddress = Console.ReadLine();

    //double latitude;
    //Console.WriteLine("Enter latitude:");
    //while (!double.TryParse(Console.ReadLine(), out latitude))
    //{
    //    Console.WriteLine("Invalid input. Please enter a valid latitude:");
    //}

    //double longitude;
    //Console.WriteLine("Enter longitude:");
    //while (!double.TryParse(Console.ReadLine(), out longitude))
    //{
    //    Console.WriteLine("Invalid input. Please enter a valid longitude:");
    //}

    DateTime timeOpened;
    Console.WriteLine("Enter time opened (YYYY-MM-DD HH:mm:ss):");
    if (!DateTime.TryParse(Console.ReadLine(), out timeOpened))
    {
        throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time (YYYY-MM-DD HH:mm:ss):");
    }

    DateTime? maxTimeToClose = null;
    Console.WriteLine("Enter max time to close (or leave empty):");
    string maxTimeInput = Console.ReadLine();
    if (!string.IsNullOrEmpty(maxTimeInput) && !DateTime.TryParse(maxTimeInput, out DateTime parsedMaxTime))
    {
        throw new BO.BlWrongInputException("Invalid input. Please enter a valid date and time for max time to close.");
    }

    //BO.StatusTreat status;
    ////Console.WriteLine("Enter status (   Open,  Treat,  Close, Expired,   RiskOpen,  TreatInRisk):");
    //while (!Enum.TryParse(Console.ReadLine(), out status) || !Enum.IsDefined(typeof(BO.StatusTreat), status))
    //{
    //    Console.WriteLine("Invalid input. Please enter a valid status (0 for Pending, 1 for InProgress, 2 for Completed):");
    //}

    return new BO.Call
    {
        ID = id,
        callT = callType,
        verbalDescription = description,
        address = fullAddress,
        latitude = 0,
        longitude = 0,
        openTime = timeOpened,
        maxTime = maxTimeToClose,
        statusC = 0
    };

}
}