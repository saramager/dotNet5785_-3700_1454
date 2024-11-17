using Dal;
using DalApi;
using DO;
namespace DalTest;

public enum OPTION
{
    EXIT,
    VOLUNTEER,
    ASSIGNMENT,
    CALL,
    INIT_DB,
    SHOW_ALL_DB,
    CONFIG,
    RESET_DB
}
public enum CRUD
{
    EXIT,
    CREATE,
    READ,
    READ_ALL,
    UPDATE,
    DELETE,
    DELETE_ALL
}
public enum CONFIG
{
    EXIT,
    FORWARD_CLOCK_ONE_MINUTE,
    FORWARD_CLOCK_ONE_HOUR,
    FORWARD_CLOCK_ONE_DAY,
    FORWARD_CLOCK_ONE_MONTH,
    FORWARD_CLOCK_ONE_YEAR,
    GET_CLOCK,
    SET_MAX_RANGE,
    GET_MAX_RANGE,
    RESET_CONFIG
}

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1


    static void Main(string[] args)
    {
        try
        {
            OPTION option = showMainMenu();

            while (OPTION.EXIT != option)
            {
                
                if (OPTION.RESET_DB == option)
                {
                    s_dalVolunteer?.DeleteAll();//stage 1

                    s_dalCall?.DeleteAll(); //stage 1 
                    s_dalAssignment?.DeleteAll();//stage 1 
                    s_dalConfig?.Reset(); //stage 1 
                }
                else if (OPTION.INIT_DB == option)
                {
                    Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig); //stage 1
                   
                }
                else if (OPTION.CONFIG == option)
                {
                    handleConfigOptions();

                }
                else if (OPTION.SHOW_ALL_DB == option)
                {
                    showAllDB();
                }
                else
                {
                    handleCRUDOptions(option);
                }

                option = showMainMenu();
            }

        }
        catch (Exception ex)
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
1 - Volunteer
2 - Assignment
3 - Call
4 - Init DB
5 - Show all database
6 - Config
7 - Reset DB & Config");
   
        }
        while (!int.TryParse(Console.ReadLine(), out  choice));
        return (OPTION)choice;
    }

    private static void handleConfigOptions()
    {
        try
        {
            switch (showConfigMenu())
            {
                case CONFIG.FORWARD_CLOCK_ONE_MINUTE:
                    {
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_HOUR:
                    {
                       s_dalConfig!.Clock =s_dalConfig.Clock.AddHours(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_DAY:
                    {
                       s_dalConfig!.Clock =s_dalConfig.Clock.AddDays(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_MONTH:
                    {
                       s_dalConfig!.Clock =s_dalConfig.Clock.AddMonths(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_YEAR:
                    {
                       s_dalConfig!.Clock =s_dalConfig.Clock.AddYears(1);
                        break;
                    }
                case CONFIG.GET_CLOCK:
                    {
                        Console.WriteLine(s_dalConfig!.Clock);
                        break;
                    }
                case CONFIG.SET_MAX_RANGE:
                    {
                        Console.Write("enter Max Range: ");
                       
                        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan  riskRange))
                            throw new FormatException("Wrong input");
                       s_dalConfig!.RiskRange = riskRange;
                        break;
                    }
                case CONFIG.GET_MAX_RANGE:
                    {
                        Console.WriteLine(s_dalConfig!.RiskRange);
                        break;
                    }
                case CONFIG.RESET_CONFIG:
                   s_dalConfig!.Reset();
                    break;
                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    private static CONFIG showConfigMenu()
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
Config Options:
0 - Exit
1 - Forward Clock One Hour
2 - Forward Clock One Day
3 - Forward Clock One Month
4 - Forward Clock One Year
5 - Get Clock
6 - Set RiskRange
7 - Get RiskRange 
8 - ResetConfig Config");
        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (CONFIG)choice;
    }
    private static void showAllDB()
    {
        Console.WriteLine("--------------- List of Volunteers ------------------------------------------");
        foreach (var item in s_dalVolunteer!.ReadAll())
        {
            Console.WriteLine(item);
        }
        Console.WriteLine("--------------- List of Calls ------------------------------------------");
        foreach (var item in s_dalCall!.ReadAll())
        {
            Console.WriteLine(item);
        }
        Console.WriteLine("--------------- List of Assignments ------------------------------------------");
        foreach (var item in s_dalAssignment!.ReadAll())
        {
            Console.WriteLine(item);
        }
    }
    private static void handleCRUDOptions(OPTION entity)
    {
        try
        {
           
            switch (showCrudMenu(entity))
            {
                case CRUD.CREATE:
                    handleCreate(entity);
                    break;
                case CRUD.READ:
                    handleRead(entity);
                    break;
                case CRUD.READ_ALL:
                    handleReadAll(entity);
                    break;
                case CRUD.UPDATE:
                    handleUpdate(entity);
                    break;
                case CRUD.DELETE:
                    handleDelete(entity);
                    break;
                case CRUD.DELETE_ALL:
                    handleDeleteAll(entity);
                    break;

                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static CRUD showCrudMenu(OPTION entity)
    {
        int choice;
        do
        {
            Console.WriteLine(@$"
{entity} CRUD Options:
0 - Exit
1 - Create 
2 - Read
3 - ReadAll
4 - Update 
5 - Delete
6 - Delete All");
        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (CRUD)choice;
    }

    private static void handleCreate(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                createVolunteer(out Volunteer vo);
                s_dalVolunteer!.Create(vo);
                break;
            case OPTION.ASSIGNMENT:
                createAssignment(out Assignment ass);
                s_dalAssignment!.Create(ass);
                break;
            case OPTION.CALL:
                createCall(out Call ca);
                s_dalCall!.Create(ca);
                break;
            default:
                break;
        }
    }
    private static void handleRead(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dalVolunteer!.Read(id));
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dalAssignment!.Read(id));
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dalCall!.Read(id));
                break;
            default:
                break;
        }
    }

    private static void handleReadAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                foreach (var item in s_dalVolunteer!.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.ASSIGNMENT:
                foreach (var item in s_dalAssignment!.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.CALL:
                foreach (var item in s_dalCall!.ReadAll())
                    Console.WriteLine(item);
                break;
            default:
                break;
        }
    }

    private static void handleUpdate(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                Console.WriteLine(s_dalVolunteer!.Read(id));
                createVolunteer(out Volunteer vo, id);
                s_dalVolunteer!.Update(vo);
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dalAssignment!.Read(id));
                createAssignment(out Assignment ass, id);
                s_dalAssignment!.Update(ass);
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dalCall!.Read(id));
                createCall(out Call ca, id);
                s_dalCall!.Update(ca);
                break;
            default:
                break;
        }
    }
    private static void handleDelete(OPTION entity)
    {
        Console.WriteLine("Enter an id");
        if (false == int.TryParse(Console.ReadLine(), out int id))
            Console.WriteLine("Wrong input");

        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dalVolunteer!.Delete(id);
                break;
            case OPTION.ASSIGNMENT:
                s_dalAssignment!.Delete(id);
                break;
            case OPTION.CALL:
                s_dalCall!.Delete(id);
                break;
            default:
                break;
        }
    }

    private static void handleDeleteAll(OPTION entity)
    {
        switch (entity)
        {
            case OPTION.VOLUNTEER:
                s_dalVolunteer!.DeleteAll();
                break;
            case OPTION.ASSIGNMENT:
                s_dalAssignment!.DeleteAll();
                break;
            case OPTION.CALL:
                s_dalCall!.DeleteAll();
                break;
            default:
                break;
        }
    }

    private static void createVolunteer(out Volunteer vo, int id = 0)
    {
        double? latitude;
        double? longitude;
        double? maxDis;
        
        if (id == 0)
        {
            Console.Write("enter VolunteerId: ");
            if (!int.TryParse(Console.ReadLine(), out int _id))
                throw new FormatException("Wrong input");
            else
                id = _id;
        }
        
        Console.Write("enter Name of the Volunteer: ");
        string? name = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("enter phone of the Volunteer: ");
        string? phone = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("enter email of the Volunteer: ");
        string? email = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("enter true/false if the Volunteer is active: ");
        if (!bool.TryParse(Console.ReadLine(), out bool active))
            throw new FormatException("Wrong input");

        Console.Write("Enter role of the Volunteer: Manager or TVolunteer: ");
        if (!RoleType.TryParse(Console.ReadLine(), out RoleType role))
             throw new FormatException("Wrong input");

        Console.Write("Enter the distance type: AirDistance, walkingDistance or DrivingDistance: ");
       if (!Distance.TryParse(Console.ReadLine(), out  Distance distance))
            throw new FormatException("Wrong input- of distance ");
    
        Console.Write("Enter current address: ");
        string? address = Console.ReadLine() ?? null;

        Console.Write("Enter the Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latResult))
            latitude = null;
        else
            latitude = latResult;

        Console.Write("Enter the Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double lonResult))
            longitude = null;
        else
            longitude = lonResult;
        Console.Write("Enter max Distance: ");
        if (!double.TryParse(Console.ReadLine(), out double DisResult))
            maxDis = null;
        else
           maxDis = DisResult;



        vo = new Volunteer(id, name, phone, email, active, role, distance, "password321",address, latitude, longitude,maxDis);
    }
        

    private static void createAssignment(out Assignment ass, int id = 0)
    {
        DateTime? finishTime;
        FinishType? finishType;

            Console.Write("enter Call Id: ");
            if (!int.TryParse(Console.ReadLine(), out int Cid))
                throw new FormatException("Wrong input");
         
            Console.Write("enter Volunteer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int Vid))
                throw new FormatException("Wrong input");

        Console.Write("enter the start treatment: ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
            throw new FormatException("Wrong input");

        Console.Write("Enter the finish time: ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime finishT))
            finishTime = null;
        else
            finishTime = finishT;

        Console.Write("Enter the finish type: ");
        if (!FinishType.TryParse(Console.ReadLine(), out FinishType typeF))
            finishType = null;
        else
            finishType = typeF;

        ass = new Assignment(id, Cid, Vid, start, finishTime, finishType);



    }
    private static void createCall(out Call ca, int id = 0)
    {
        DateTime? maxTime;
   
        Console.Write("Enter the Call address: ");
        string? address = Console.ReadLine() ?? throw new FormatException("Wrong input");


        Console.Write("Enter the call type: ");
   
        if (!CallType.TryParse(Console.ReadLine(), out CallType typeCall))
            throw new FormatException("Wrong input- of CallType");

        Console.Write("Enter the Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
            throw new FormatException("Wrong input- of Latitude");

        Console.Write("Enter the Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
            throw new FormatException("Wrong input- of Longitude");

        Console.Write("Enter open time for call: ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
            throw new FormatException("Wrong input- of Time");

        Console.Write("Enter the maximum time for Call: ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime MaxResult))
            maxTime = null;
        else
            maxTime = MaxResult;

        Console.Write("Enter the verbal describe  ");
        string? describe = Console.ReadLine() ?? null;

        ca =  new Call(id ,address,typeCall ,latitude,longitude, openTime,maxTime,describe);
    }




}





