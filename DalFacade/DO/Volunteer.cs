
namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="ID">ID number that uniquely identifies the volunteer.>
/// <param name="fullName">Volunteer's First name and last name >
/// <param name="phone">Stands for a standard cell phone. 10 digits only. Starts 0.>
/// <param name="email">Valid email address>
/// <param name="password">Personal password for a volunteer>
/// <param name="currentAddress">Full and real address in correct format of the volunteer.>
/// <param name="Latitude">A number indicating how far a point on the Earth is south or north of the equator.>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator.>
/// <param name="role">"Manager" or "Volunteer">
/// <param name="active">Is the volunteer active or inactive (retired from the organization).>
/// <param name="maxDistance">Each volunteer will define through the display the maximum distance for receiving a call.>
/// <param name="distanceType">Aerial distance, walking distance, driving distance The default is air distance.>
public record Volunteer
(
    int ID,
    string fullName,
    string phone,
    string email,
    bool active,
    RoleType role,
    Distance distanceType,
    string? password = null,
    string? currentAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    double? maxDistance = null
 )

{
    public Volunteer() : this(0,"","","",default, RoleType.TVolunteer, Distance.AirDistance) { } // empty ctor for stage 3 
}
