namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
    // המערך של הכתובות באנגלית
    public static readonly string[] addresses = new string[]
    {
        "Amram Gaon 14, Jerusalem, Israel",
        "Amram Gaon 6, Jerusalem, Israel",
        "Harav Haim Vital 8, Jerusalem, Israel",
        "Beit Hadfus 9, Jerusalem, Israel",
        "Harav Shrira Gaon 18, Jerusalem, Israel",
        "Harav Shrira Gaon 10, Jerusalem, Israel",
        "Harav Shrira Gaon 5, Jerusalem, Israel",
        "Beit Hadfus 22, Jerusalem, Israel",
        "Beit Hadfus 35, Jerusalem, Israel",
        "Kanfei Nesharim 13, Jerusalem, Israel",
        "Kanfei Nesharim 24, Jerusalem, Israel",
        "Kanfei Nesharim 40, Jerusalem, Israel",
        "Harav Tzvi Yehuda 16, Jerusalem, Israel",
        "Najara 3, Jerusalem, Israel",
        "HaIlui 9, Jerusalem, Israel",
        "Harav Reines 30, Jerusalem, Israel",
        "Najara 14, Jerusalem, Israel",
        "Givat Shaul 25, Jerusalem, Israel",
        "Beit Shearim 4, Jerusalem, Israel",
        "Beit Hadfus 22, Jerusalem, Israel"
    };

    // (Longitude) מערך קווי האורך
    public static readonly double[] longitudes = new double[]
    {
        35.19262, 35.19285, 35.19285, 35.18921, 35.19136,
        35.19252, 35.19383, 35.18556, 35.18081, 35.18475,
        35.1867, 35.18392, 35.19622, 35.19214, 35.19566,
        35.195, 35.19139, 35.19087, 35.19624, 35.18556
    };

    // (Latitude) מערך קווי הרוחב
    public static readonly double[] latitudes = new double[]
    {
        31.79052, 31.79141, 31.78845, 31.7857, 31.78878,
        31.78846, 31.78827, 31.78659, 31.78582, 31.78737,
        31.78813, 31.78792, 31.78776, 31.79202, 31.78888,
        31.79003, 31.79062, 31.79251, 31.79032, 31.78659
    };
    private static void createVolunteers()
    {

        string[] names = { "David Levy", "Miriam Cohen", "Yosef Katz", "Sarah Gold", "Shlomo Klein", "Chana Rosen", "Aharon Azulay", "Tova Benita", "Yaakov Pinto", "Esther Bar", "Dov Cohen", "Rivka Dayan", "Yitzhak Azulay", "Malkah Shalom", "Avraham Segal" ,"Moshe Chaim"};
        string[] emails = { "levy.david@email.com", "miriamCohen@gmail.com", "yosef.katz@outlook.com", "sarahGold789@yahoo.com", "shlomo.klein@live.com", "chana.rosen@icloud.com", "aharonAzulay@mail.com", "tova.benita@webmail.com", "yaakov.pinto@zoho.com", "esther.bar@aol.com", "dov.cohen@ymail.com", "rivka.dayan@fastmail.com", "yitzhak.azulay@protonmail.com", "malkah.shalom@gmx.com", "avraham.segal@tutanota.com","moshe342@gmail.co.il" };
        string[] phoneNumbers = { "0521234567", "0532345678", "0543456789", "0554567890", "0585678901", "0596789012", "0577890123", "0588901234", "0529012345", "0530123456", "0541234567", "0552345678", "0583456789", "0574567890", "0595678901", "050897463" };
        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            string email = emails[i];
            string phone = phoneNumbers[i];
            int VId = s_rand.Next(200000000, 400000001);
            bool active = !(i % 7 == 0);
            double DisMax = s_rand.NextDouble() * 5.0;
            int randAddress = s_rand.Next(addresses.Length);
            while (s_dalVolunteer!.Read(VId) != null)
            {
                 VId = s_rand.Next(200000000, 400000001);
            }
            s_dalVolunteer!.Create(new Volunteer(VId, name, phone, email, active, (i!= names.Length-1)?RoleType.TVolunteer:RoleType.Manager, Distance.AirDistance, "password321", addresses[randAddress],latitudes[randAddress],longitudes[randAddress] ,DisMax));
        }
    }
    private static void createAssignment()
    {
        for (int i = 0; i < 60; i++)
        {
           
            int randVolunteer = s_rand.Next(s_dalVolunteer!.ReadAll().Count);
            Volunteer volunteerToAssig = s_dalVolunteer.ReadAll()[randVolunteer];
            int randCAll = s_rand.Next(s_dalCall!.ReadAll().Count - 15);
            Call callToAssig = s_dalCall.ReadAll()[randCAll];
            while (callToAssig.openTime> s_dalConfig!.Clock)
            {
                randCAll = s_rand.Next(s_dalCall!.ReadAll().Count - 15);
                callToAssig = s_dalCall.ReadAll()[randCAll];
            }
            FinishType? finish= null;
            DateTime? finishTime= null;
            if (callToAssig.maxTime!=null&& callToAssig.maxTime>= s_dalConfig?.Clock)
            {
                finish = FinishType.ExpiredCancel;
            }
            else
            {
                int randFinish = s_rand.Next(0,4);
                switch (randFinish)
                {
                    case 0: finish = FinishType.Treated;
                        finishTime = s_dalConfig!.Clock;
                        break;
                    case 1: finish = FinishType.SelfCancel; break;
                    case 2: finish = FinishType.ManagerCancel; break;
               



                }
            }
            s_dalAssignment?.Create(new Assignment(0,callToAssig.id,volunteerToAssig.ID, s_dalConfig!.Clock, finishTime,finish));  


        }

    }
    private static void createCall()
    {
        string[][] VolunteerDescriptions = new string[][] {
            // BabyGift descriptions
            new string[]
            {
            "A family is expecting a baby girl and is in need of some warm clothes for her first few months. You could provide a set of onesies, socks, and a soft blanket to help them prepare.",
            "A new mother is organizing a baby shower and would appreciate a thoughtful gift for her newborn girl. Consider giving a cute outfit or a set of baby care essentials, like lotions and diapers.",
            "A friend recently had a baby girl and could use some baby clothes. A small set of soft cotton onesies, mittens, and a cozy baby hat would make a perfect gift.",
            "The family has a baby girl, and they need some practical clothing. A set of baby shirts, pants, and a blanket would be greatly appreciated during the first few weeks of the baby's life.",
            "The new parents have everything they need except for clothes for their baby girl. A gift box with onesies, socks, and baby booties would be a nice and practical gesture."
            },
        
        // MomGift descriptions
        new string[]
        {
            "A mother of twins who gave birth last week is overwhelmed and in need of personal care items. You could provide soaps, lotions, and a bath kit to help her feel more comfortable during her recovery.",
            "A new mother just had twins and is struggling to find time for self-care. You could help by giving her a pampering gift set with soothing creams, oils, and nursing pads to help her recover.",
            "The new mom is in need of items that will help her feel better after a difficult delivery. A thoughtful care package with postpartum essentials like breast pads, bath salts, and chocolate would be a great gift.",
            "A new mom with twins is exhausted and could really use some personal care items to feel refreshed. Consider gifting her some relaxing bath essentials like body wash, lotions, and a towel set.",
            "After giving birth to twins, a mother could use a little pampering. A set of lotions, oils, and some comfortable pajamas or slippers would help her feel more comfortable while recovering at home."
        },
        
        // HouseholdAssistance descriptions
        new string[]
        {
            "A new family with a newborn is having trouble keeping the house organized. You could volunteer to help tidy up the nursery, fold baby clothes, and organize baby gear.",
            "The new parents are overwhelmed with housework and could really use a hand. Offering to clean the kitchen or do the laundry can be a huge help to relieve some of the burden.",
            "A friend with a newborn has a lot of things to do around the house but lacks the time. You could volunteer to run errands, such as picking up groceries, or help with organizing the baby’s items.",
            "A mother is struggling to manage her older children and a newborn. You could help by looking after the older kids for a couple of hours so the mom can rest or focus on the baby.",
            "The new parents are so busy with their newborn that they haven’t been able to keep up with basic chores. Offering to clean the house, help with groceries, or cook a simple meal would be much appreciated."
        },

        // MealPreparation descriptions
        new string[]
        {
            "A family with two parents and four kids is struggling to make dinner every night. You could help by preparing a dairy supper of pasta, a simple salad, and bread for the family.",
            "The new parents have a large family and need a break from cooking. You could prepare a large, easy-to-make meal, like mac and cheese, roasted vegetables, and some garlic bread to feed everyone.",
            "A family with young kids is in need of meals to help ease their busy days. You could prepare a large batch of soup, sandwiches, and fresh fruit for them to enjoy throughout the week.",
            "The family needs a kosher meal, so you could prepare a dinner that complies with their dietary restrictions. A simple kosher chicken dish, rice, and a vegetable side would be perfect for the family of six.",
            "A mother with four kids is busy with household duties and would appreciate a meal prepared for her family. You could make a hearty chicken soup, some bread, and a salad to nourish the whole family.",
            "A family following strict kosher guidelines is in need of a meal. Prepare a kosher meal with BD”C certification, such as a kosher meat dish, rice, and a vegetable side dish, ensuring everything is compliant with their dietary needs.",
            "A large family is following a kosher diet and would appreciate a prepared meal. You could cook a kosher meal with the OU certification, such as a roast chicken, potatoes, and a fresh salad, making sure all ingredients are certified kosher."
        }

    };
        DateTime startRange = new DateTime(s_dalConfig!.Clock.Year, s_dalConfig.Clock.Month, 1); //form the beginning for the month
        int range = (s_dalConfig!.Clock - startRange).Hours; //num fo days intill to day
        for (int i = 0; i < 60; i++)
        {
            CallType TypeOfCall = (CallType)s_rand.Next(0, 4);  // Randomly select a volunteer activity
            int randAddress = s_rand.Next(addresses.Length);// Randomly selsct adrees of call
            string CallDescription = VolunteerDescriptions[(int)TypeOfCall][s_rand.Next(0, VolunteerDescriptions[(int)TypeOfCall].Length)];

            DateTime startCall = startRange.AddHours(s_rand.Next(range));
            DateTime? finishCall = null;
            int CallTime = (s_dalConfig!.Clock - startCall).Hours; //num fo hours forn the start 
            if (i % 10 == 0)
            { finishCall = startCall.AddHours(CallTime - 1); }
            else if (s_rand.Next(0, 2) == 1)
            {
                finishCall = startCall.AddHours(CallTime + s_rand.Next(200));
            }
            s_dalCall!.Create(new Call(0,addresses[randAddress], TypeOfCall, latitudes[randAddress], longitudes[randAddress], startCall, finishCall, CallDescription));

        }

    }
    public static void Do( IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL can not be null!");
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL can not be null!");
        Console.WriteLine( "Reset Configuration values and List values.");
        s_dalConfig.Reset(); //stage 1
        s_dalAssignment.DeleteAll(); //stage 1
        s_dalCall.DeleteAll();
        s_dalVolunteer.DeleteAll();
                                  
        Console.WriteLine("Initializing Volunteer list");
        createVolunteers();
        Console.WriteLine("Initializing Call list");
        createCall();
        Console.WriteLine("Initializing Assignment list");
        createAssignment();
    }
}




