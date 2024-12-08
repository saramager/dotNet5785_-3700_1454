using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;

/// <summary>
/// 
/// </summary>
public interface IVolunteer
{

    /// <summary>Validates the user login by username and password, and returns the user's role.</summary>
    /// <param name="name">The username of the user attempting to log in.</param>
    /// <param name="password">The password of the user attempting to log in.</param>
    /// <returns>The role of the user if login is successful.</returns> 
    RoleType EnterToSystem (string name, string password );
    /// <summary>Filters and sorts the volunteer list based on the provided parameters.</summary>
    /// <param name="IsActive">A nullable boolean to filter by active/inactive volunteers. If null, no filtering is applied.</param>
    /// <param name="filedToSort">A nullable enum to sort by a specific field. If null, sorts by ID (Tz).</param>
    /// <returns>A filtered and sorted collection of "VolunteerInList" entities.</returns>
    IEnumerable<VolunteerInList> GetVolunteerInList( bool? IsActive, BO.FiledOfVolunteerInList? filedToSort  );
    Volunteer ReadVolunteer  (int id);
    void UpdateVolunteer(int id, Volunteer vol);
    void DeleteVolunteer(int id);
    void CreateVolunteer (Volunteer volToAdd);

}
