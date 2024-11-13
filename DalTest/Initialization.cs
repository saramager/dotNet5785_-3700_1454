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
    private static  void createVolunteers()
    {
        string[] names = { "David Levy", "Miriam Cohen", "Yosef Katz", "Sarah Gold", "Shlomo Klein", "Chana Rosen", "Aharon Azulay", "Tova Benita", "Yaakov Pinto", "Esther Bar", "Dov Cohen", "Rivka Dayan", "Yitzhak Azulay", "Malkah Shalom", "Avraham Segal" };
        string[] emails = { "david.levy@email.com", "miriam.cohen@gmail.com", "yosef.katz@outlook.com", "sarah.gold@yahoo.com", "shlomo.klein@live.com", "chana.rosen@icloud.com", "aharon.azulay@mail.com", "tova.benita@webmail.com", "yaakov.pinto@zoho.com", "esther.bar@aol.com", "dov.cohen@ymail.com", "rivka.dayan@fastmail.com", "yitzhak.azulay@protonmail.com", "malkah.shalom@gmx.com", "avraham.segal@tutanota.com" };
        string[] phoneNumbers = { "0521234567", "0532345678", "0543456789", "0554567890", "0585678901", "0596789012", "0577890123", "0588901234", "0529012345", "0530123456", "0541234567", "0552345678", "0583456789", "0574567890", "0595678901" };
        string adress[] = {}
        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            string email = emails[i];
            string phone = phoneNumbers[i];

        }
};



    };


    }


