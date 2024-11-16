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
                    //Initialization.Do(s_dal); //stage 2
                    //Initialization.Do(); //stage 4
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
        while (!int.TryParse(Console.ReadLine(), out choice));
        return (OPTION)choice;
    }

    private static void handleConfigOptions()
    {
        try
        {
            switch (showConfigMenu())
            {
                case CONFIG.FORWARD_CLOCK_ONE_HOUR:
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_DAY:
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_MONTH:
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                        break;
                    }
                case CONFIG.FORWARD_CLOCK_ONE_YEAR:
                    {
                        s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                        break;
                    }
                case CONFIG.GET_CLOCK:
                    {
                        Console.WriteLine(s_dal.Config.Clock);
                        break;
                    }
                case CONFIG.SET_MAX_RANGE:
                    {
                        Console.Write("enter Max Range: ");
                        if (!int.TryParse(Console.ReadLine(), out int maxRange))
                            throw new FormatException("Wrong input");
                        s_dal.Config.MaxRange = maxRange;
                        break;
                    }
                case CONFIG.GET_MAX_RANGE:
                    {
                        Console.WriteLine(s_dal.Config.MaxRange);
                        break;
                    }
                case CONFIG.RESET_CONFIG:
                    s_dal.Config.Reset();
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
6 - Set MaxRange
7 - Get MaxRange 
8 - ResetConfig Config");
        }
        while (!int.TryParse(s: Console.ReadLine(), out choice));
        return (CONFIG)choice;
    }
    private static void showAllDB()
    {
        Console.WriteLine("--------------- List of Courses ------------------------------------------");
        foreach (var item in s_dal!.ASSIGNMENT.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Students ------------------------------------------");
        foreach (var item in s_dal!.VOLUNTEER.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Links ------------------------------------------");
        foreach (var item in s_dal!.CALL.ReadAll())
        {
            Console.WriteLine(item);
        }
    }
    private static void handleCRUDOptions(OPTION entity)
    {
        try
        {
            //if (OPTION.SHOW_ALL_DB == option)
            //{
            //    showAllDB();
            //    return;
            //}
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
                s_dal!.Volunteer.Create(vo);
                break;
            case OPTION.ASSIGNMENT:
                createAssignment(out Assignment ass);
                s_dal!.Assignment.Create(ass);
                break;
            case OPTION.CALL:
                createCall(out Call ca);
                s_dal!.Call.Create(ca);
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
                Console.WriteLine(s_dal!.Volunteer.Read(id));
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal!.Assignment.Read(id));
                break;
            case OPTION.CALL:
                //removed - from 2025
                //Console.WriteLine("Enter the next id");
                //if (false == int.TryParse(Console.ReadLine(), out int id2))
                //    Console.WriteLine("Wrong input");
                //Console.WriteLine(s_dal!.Link.Read(id, id2));
                Console.WriteLine(s_dal!.Call.Read(id));
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
                foreach (var item in s_dal!.Volunteer.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.ASSIGNMENT:
                foreach (var item in s_dal!.Assignment.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.CALL:
                foreach (var item in s_dal!.Call.ReadAll())
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
                Console.WriteLine(s_dal!.Volunteer.Read(id));
                createVolunteer(out Volunteer vo, id);
                s_dal!.Volunteer.Update(vo);
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal!.Assignment.Read(id));
                createAssignment(out Assignment ass, id);
                s_dal!.Assignment.Update(ass);
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dal!.Call.Read(id));
                createCall(out Call ca, id);
                s_dal!.Call.Update(ca);
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
                s_dal!.Volunteer.Delete(id);
                break;
            case OPTION.ASSIGNMENT:
                s_dal!.Assignment.Delete(id);
                break;
            case OPTION.CALL:
                s_dal!.Call.Delete(id);
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
                s_dal!.Volunteer.DeleteAll();
                break;
            case OPTION.ASSIGNMENT:
                s_dal!.Assignment.DeleteAll();
                break;
            case OPTION.CALL:
                s_dal!.Call.DeleteAll();
                break;
            default:
                break;
        }
    }

    private static void createVolunteer(out Volunteer vo, int id = 0)
    {
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

        Console.Write("enter true/false if the Student is active: ");
        if (!bool.TryParse(Console.ReadLine(), out bool active))
            throw new FormatException("Wrong input");

        Console.Write("Enter role of the Volunteer: ");
        int input1 = int.Parse(Console.ReadLine());

        // בדיקה ישירה של הערכים
        if (input1 == (int)RoleType.Manager || input1 == (int)RoleType.TVolunteer)
        {
            // המרה של הערך ל-`RoleType`
            RoleType role = (RoleType)input1;
        }
        else
            throw new FormatException("Invalid input: must be 0 or 1.");
        // Distance distanceType?

        Console.Write("Enter the distance type: ");
        int input2 = int.Parse(Console.ReadLine());

        // בדיקה ישירה של הערכים
        if (input2 == (int)Distance.AirDistance || input2 == (int)Distance.walkingDistance
            || input2 == (int)Distance.DrivingDistance)
        {
            // המרה של הערך ל-`Distance`
            Distance distance = (Distance)input2;
        }
        else
            throw new FormatException("Invalid input: must be 0 or 1 or 2.");

        vo = new Volunteer(id, name, phone, email, active, role, distance);
    }
        

    private static void createAssignment(out Assignment ass, int id = 0)
    {
       
        Console.Write("enter id of the call: ");
       /// איך עושים?

        if (id == 0)
        {
            Console.Write("enter VolunteerId: ");
            if (!int.TryParse(Console.ReadLine(), out int _id))
                throw new FormatException("Wrong input");
            else
                id = _id;
        }

        ass = new Assignment(0, 0 ,id , default(DateTime));



    }
    private static void createCall(out Call ca, int id = 0)
    {

        Console.Write("Enter the Call address: ");
        string? address = Console.ReadLine() ?? throw new FormatException("Wrong input");

        Console.Write("Enter the call type: ");
        int input = int.Parse(Console.ReadLine());

        // בדיקה ישירה של הערכים
        if (input == (int)CallType.BabyGift || input == (int)CallType.MomGift
            || input == (int)CallType.HouseholdHelp || input == (int)CallType.MealPreparation)
        {
            // המרה של הערך ל-`CallType`
            CallType typeCall = (CallType)input;
        }
        else
            throw new FormatException("Invalid input: must be 0 or 1 or 2 or 3.");
        //קווי אורך
        //קוורי רוחב


        ca = Call(0,address,typeCall, , , default(DateTime));
    }



}





