
using DalApi;
namespace Dal;
/// <summary>
/// Implementation of IDal based on lists in memory:
/// DalList is responsible for accessing data stored in Lists
/// </summary>
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IAssignment Assignment  { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    //public IAssignment Assignment => throw new NotImplementedException();

    //public ICall Call => throw new NotImplementedException();

    //public IVolunteer Volunteer => throw new NotImplementedException();

    //public IConfig Config => throw new NotImplementedException();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
