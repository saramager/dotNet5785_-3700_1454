
namespace BO;

public enum CallType
{
    BabyGift,
    MomGift,
    HouseholdHelp,
    MealPreparation,
    None
}
/// <summary>
/// for Volunteer class - this enum represent the role  manger or regular volunteer 
/// </summary>
public enum RoleType
{

    Manager,
    Volunteer
}
/// <summary>
///  for Call class - this enum represent the  different types of distance: Air distance, walking distance, Driving distance.
/// </summary>
public enum Distance
{
    AirDistance,
    walkingDistance,
    DrivingDistance

}
/// <summary>
/// for Assignment  class - this enum represent the stage for the assignment ist she treated, self cancel (by the volunteer), manager cancel (by the manger ) or expired call (the time ends )    manger or regular volunteer 
/// </summary>
public enum FinishType
{
    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}

public enum Status
{
    Open,
    InTreat,
    Close,
    Expired,
    OpenInRisk,
    TreatInRisk
}
public  enum FiledOfVolunteerInList
{
    ID , fullName ,active , numCallsHandled ,numCallsCancelled ,numCallsExpired,CallId , callT

}
public enum FiledOfCallInList
{
    ID ,CallId , callT,openTime,timeEndCall , volunteerLast ,TimeEndTreat ,status , numOfAssignments

}
public enum FiledOfClosedCallInList
{ ID, address, callT, openTime, startTreatment, finishTreatment, finishT, CallDistance }
public enum FiledOfOpenCallInList
{
    ID, address , callT ,openTime , maxTime ,verbalDescription

}
public enum TimeUnit
{
    Minute, Hour, Day, Month, Year
}