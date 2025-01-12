namespace DO;

public enum CallType
{
    BabyGift,
    MomGift,
    HouseholdHelp,
    MealPreparation

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