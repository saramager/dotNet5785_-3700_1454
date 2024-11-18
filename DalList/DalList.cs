namespace Dal;
using DalApi;
/// <summary>
/// using the IDal to unionid all the lists
/// </summary>
sealed public class DalList : IDal
{
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
