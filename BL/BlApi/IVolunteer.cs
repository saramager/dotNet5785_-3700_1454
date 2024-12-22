using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;

/// <summary>
/// intterface for Vlounteer 
/// </summary>
public interface IVolunteer: IObservable
{

    /// <summary>Validates the user login by username and password, and returns the user's role.</summary>
    /// <param name="id">The username of the user attempting to log in the id .</param>
    /// <param name="password">The password of the user attempting to log in.</param>
    /// <returns>The role of the user if login is successful.</returns> 
    BO.RoleType EnterToSystem (int id, string password );
    /// <summary>Filters and sorts the volunteer list based on the provided parameters.</summary>
    /// <param name="IsActive">A nullable boolean to filter by active/inactive volunteers. If null, no filtering is applied.</param>
    /// <param name="filedToSort">A nullable enum to sort by a specific field. If null, sorts by ID (Tz).</param>
    /// <returns>A filtered and sorted collection of "VolunteerInList" entities.</returns>
  
    IEnumerable<BO.VolunteerInList> GetVolunteerInList( bool? IsActive, BO.FiledOfVolunteerInList? filedToSort  );
    /// <summary>
    /// Retrieves a volunteer by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>A <see cref="BO.Volunteer"/> object representing the volunteer with the specified ID.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when no volunteer exists with the specified ID.</exception>
    BO.Volunteer ReadVolunteer  (int id);
    /// <summary>
    /// Updates the information of a volunteer.
    /// </summary>
    /// <param name="id">
    /// The ID of the volunteer to be updated.
    /// </param>
    /// <param name="vol">
    /// The volunteer object containing the updated data.
    /// </param>
    /// <exception cref="BO.VolunteerCantUpadeOtherVolunteerException">
    /// Thrown when attempting to update another volunteer's data without proper permissions.
    /// </exception>
    /// <exception cref="CantUpdatevolunteer">
    /// Thrown when a volunteer cannot be updated due to specific constraints (e.g., open assignments or invalid role changes).
    /// </exception>
    /// <exception cref="BO.BlDoesNotExistException">
    /// Thrown when the volunteer does not exist in the database.
    /// </exception>
    void UpdateVolunteer(int id, BO.Volunteer vol);
    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">
    /// The ID of the volunteer to be deleted.
    /// </param>
    /// <exception cref="Exception">
    /// Thrown if the volunteer has open assignments and cannot be deleted.
    /// </exception>
    /// <exception cref="BO.BlDoesNotExistException">
    /// Thrown when the volunteer does not exist in the database.
    /// </exception>

    void DeleteVolunteer(int id);
    /// <summary>
    /// Creates a new volunteer in the system.
    /// </summary>
    /// <param name="volToAdd">
    /// The volunteer object containing the information of the volunteer to be created.
    /// </param>
    /// <exception cref="BO.BlDoesAlreadyExistException">
    /// Thrown if a volunteer with the same ID already exists in the system.
    /// </exception>

    void CreateVolunteer (BO.Volunteer volToAdd);

}
