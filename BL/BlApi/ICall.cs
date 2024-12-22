using BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;

/// <summary>
/// 
/// </summary>
public interface ICall: IObservable
{
    /// <summary>
    /// Calculates the sum of calls for each status.
    /// </summary>
    /// <returns>An array where each index represents a status and its value represents the count of calls with that status.</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if there are no calls in the database.</exception>
    int[] SumOfCalls();

    /// <summary>
    /// Retrieves a list of calls based on optional filters and sorting parameters.
    /// </summary>
    /// <param name="filedToFilter">The field by which to filter the calls (e.g., ID, CallId, callT, openTime, etc.).</param>
    /// <param name="sort">The value to filter against.</param>
    /// <param name="filedToSort">The field by which to sort the calls (e.g., ID, CallId, callT, openTime, etc.).</param>
    /// <returns>An `IEnumerable<CallInList>` representing the filtered and/or sorted list of calls.</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if there are no calls in the database.</exception>
    IEnumerable< CallInList> GetCallInList (FiledOfCallInList? filedToFilter, object? sort , FiledOfCallInList? filedToSort) ;

    /// <summary>
    /// Reads a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call.</param>
    /// <returns>A `BO.Call` object with the call details and associated assignments.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist in the database.</exception>

    Call ReadCall(int id);

    /// <summary>
    /// Updates an existing call.
    /// </summary>
    /// <param name="c">The call object containing updated details.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call does not exist in the database.</exception>
    void UpdateCall(Call c);
    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    /// <exception cref="CantDeleteCallException">Thrown when the call cannot be deleted due to its status or assignments.</exception>
    void DeleteCall(int id);
    /// <summary>
    /// Creates a new call in the system.
    /// </summary>
    /// <param name="c">The call object containing the details of the new call.</param>
    void CreateCall (Call c);
    /// <summary>
    /// Retrieves a list of closed calls assigned to a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="callT">An optional call type to filter the calls.</param>
    /// <param name="filedTosort">An optional field to sort the closed calls.</param>
    /// <returns>An `IEnumerable<ClosedCallInList>` representing the list of closed calls.</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if there are no closed calls in the database.</exception>
    IEnumerable<ClosedCallInList> ReadCloseCallsVolunteer(int id, CallType? callT, FiledOfClosedCallInList? filedTosort);

    /// <summary>
    /// Retrieves a list of open calls assigned to a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="callT">An optional call type to filter the calls.</param>
    /// <param name="filedTosort">An optional field to sort the open calls.</param>
    /// <returns>An `IEnumerable<OpenCallInList>` representing the list of open calls.</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if there are no open calls in the database.</exception>
    IEnumerable<OpenCallInList> ReadOpenCallsVolunteer(int id, CallType? callT, FiledOfOpenCallInList? filedTosort);

    /// <summary>
    /// Marks an assignment's treatment as finished.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer finishing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist or cannot be updated.</exception>
    /// <exception cref="BO.VolunteerCantUpadeOtherVolunteerException">Thrown if the volunteer does not have permission to update this assignment.</exception>
    /// <exception cref="BO.AssignmentAlreadyClosedException">Thrown if the assignment is already closed.</exception>
     void FinishTreat(int volunteerId, int AssignmentId );

    /// <summary>
    /// Cancels a treatment for a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment to cancel.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment or call does not exist.</exception>
    /// <exception cref="BO.CantUpdatevolunteer">Thrown if the call is expired or already closed/cancelled.</exception>
    /// <exception cref="BO.VolunteerCantUpadeOtherVolunteerException">Thrown if the volunteer does not have permission to cancel this assignment.</exception>
    void cancelTreat(int volunteerId, int assignmentId);

    /// <summary>
    /// Choose a call for treatment by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="CallId">The ID of the call.</param>
    /// <exception cref="BO.BlValidationException">
    /// Thrown if the call is not available for treatment, if an assignment already exists,
    /// if the call does not exist, or if the call's maximum time limit has passed.
    /// </exception>
    /// <exception cref="BO.BlValidationException">
    /// Thrown if the call is already in treatment or open.
    /// </exception>
    void ChooseCallTreat(int volunteerId, int CallId);
}
