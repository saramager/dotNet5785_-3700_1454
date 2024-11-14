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
                    //s_dalStudent.DeleteAll(); //stage 1
                    ////...
                    ////s_dalCourse.DeleteAll(); //stage 1 
                    ////Links_dalLinkDeleteAll();//stage 1 
                    //s_dalConfig.ResetConfig(); //stage 1 
                }
                else if (OPTION.INIT_DB == option)
                {
                    //Initialization.Do(s_dalStudent, s_dalCourse, s_dalLink, s_dalConfig); //stage 1
                    //Initialization.Do(s_dal); //stage 2
                    Initialization.Do(); //stage 4
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
        foreach (var item in s_dal!.Course.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Students ------------------------------------------");
        foreach (var item in s_dal!.Student.ReadAll())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("--------------- List of Links ------------------------------------------");
        foreach (var item in s_dal!.Link.ReadAll())
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
                createStudent(out Student st);
                s_dal!.Student.Create(st);
                break;
            case OPTION.ASSIGNMENT:
                createCourse(out Course cr);
                s_dal!.Course.Create(cr);
                break;
            case OPTION.CALL:
                createLink(out Link lk);
                s_dal!.Link.Create(lk);
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
                Console.WriteLine(s_dal!.Student.Read(id));
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal!.Course.Read(id));
                break;
            case OPTION.CALL:
                //removed - from 2025
                //Console.WriteLine("Enter the next id");
                //if (false == int.TryParse(Console.ReadLine(), out int id2))
                //    Console.WriteLine("Wrong input");
                //Console.WriteLine(s_dal!.Link.Read(id, id2));
                Console.WriteLine(s_dal!.Link.Read(id));
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
                foreach (var item in s_dal!.Student.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.ASSIGNMENT:
                foreach (var item in s_dal!.Course.ReadAll())
                    Console.WriteLine(item);
                break;
            case OPTION.CALL:
                foreach (var item in s_dal!.Link.ReadAll())
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
                Console.WriteLine(s_dal!.Student.Read(id));
                createStudent(out Student st, id);
                s_dal!.Student.Update(st);
                break;
            case OPTION.ASSIGNMENT:
                Console.WriteLine(s_dal!.Course.Read(id));
                createCourse(out Course cr, id);
                s_dal!.Course.Update(cr);
                break;
            case OPTION.CALL:
                Console.WriteLine(s_dal!.Link.Read(id));
                createLink(out Link lk, id);
                s_dal!.Link.Update(lk);
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
                s_dal!.Student.Delete(id);
                break;
            case OPTION.ASSIGNMENT:
                s_dal!.Course.Delete(id);
                break;
            case OPTION.CALL:
                s_dal!.Link.Delete(id);
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
                s_dal!.Student.DeleteAll();
                break;
            case OPTION.ASSIGNMENT:
                s_dal!.Course.DeleteAll();
                break;
            case OPTION.CALL:
                s_dal!.Link.DeleteAll();
                break;
            default:
                break;
        }
    }
}




