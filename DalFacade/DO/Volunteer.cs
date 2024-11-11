
using System;

namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="ID"></param>
/// <param name="fullName"></param>
/// <param name="phone"></param>
/// <param name="email"></param>
/// <param name="password"></param>
/// <param name="currentAddress"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="role"></param>
/// <param name="active"></param>
/// <param name="maxDistance"></param>
/// <param name="distanceType"></param>
public record Volunteer
(
        int ID,
    string fullName,
    string phone,
    string email,
    string? password = null,
    string? currentAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    ENUM role,
    boolean active,
    double? maxDistance = null,
    ENUM distanceType
 )
{
    
    public Volunteer() : this(0) { }
}


    














}
