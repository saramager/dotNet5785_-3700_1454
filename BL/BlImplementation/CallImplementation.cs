using System;
using System.Collections.Generic;
namespace BlImplementation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    public int[] SumOfCalls()
    {
        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll()
            ?? throw new BO.BlNullPropertyException("There are no calls in the database");

        IEnumerable<BO.Call> boCalls = allCalls.Select(call => CallsManager.ConvertDOCallToBOCall(call));

        var groupedCalls = boCalls
            .GroupBy(call => call.statusC)
            .ToDictionary(group => group.Key, group => group.Count());

        int maxStatus = Enum.GetValues(typeof(BO.Status)).Cast<int>().Max();
        int[] result = new int[maxStatus + 1];

        foreach (var pair in groupedCalls)
        {
            result[(int)pair.Key] = pair.Value;
        }

        return result;
    }


    public IEnumerable<CallInList> GetCallInList(FiledOfCallInList? filedToFilter, object? sort, FiledOfCallInList? filedToSort)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");
        IEnumerable<CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallsManager.ConvertDOCallToBOCallInList(call));

        if (filedToFilter != null && sort != null)
        {
            switch (filedToFilter)
            {
                case FiledOfCallInList.ID:
                    boCallsInList = boCallsInList.Where(item => item.ID == (int)sort);
                    break;

                case FiledOfCallInList.CallId:
                    boCallsInList = boCallsInList.Where(item => item.CallId == (int)sort);
                    break;

                case FiledOfCallInList.callT:
                    boCallsInList = boCallsInList.Where(item => item.callT == (BO.CallType)sort);
                    break;

                case FiledOfCallInList.openTime:
                    boCallsInList = boCallsInList.Where(item => item.openTime == (DateTime)sort);
                    break;

                case FiledOfCallInList.timeEndCall:
                    boCallsInList = boCallsInList.Where(item => item.timeEndCall == (TimeSpan)sort);
                    break;

                case FiledOfCallInList.volunteerLast:
                    boCallsInList = boCallsInList.Where(item => item.volunteerLast == (string)sort);
                    break;

                case FiledOfCallInList.TimeEndTreat:
                    boCallsInList = boCallsInList.Where(item => item.TimeEndTreat == (TimeSpan)sort);
                    break;

                case FiledOfCallInList.status:
                    boCallsInList = boCallsInList.Where(item => item.status == (Status)sort);
                    break;

                case FiledOfCallInList.numOfAssignments:
                    boCallsInList = boCallsInList.Where(item => item.numOfAssignments == (int)sort);
                    break;
            }
        }

        if (filedToSort == null)
            filedToSort = FiledOfCallInList.CallId;

        switch (filedToSort)
        {
            case FiledOfCallInList.ID:
                boCallsInList = boCallsInList.OrderBy(item => item.ID);
                break;

            case FiledOfCallInList.CallId:
                boCallsInList = boCallsInList.OrderBy(item => item.CallId);
                break;

            case FiledOfCallInList.callT:
                boCallsInList = boCallsInList.OrderBy(item => item.callT);
                break;

            case FiledOfCallInList.openTime:
                boCallsInList = boCallsInList.OrderBy(item => item.openTime);
                break;

            case FiledOfCallInList.timeEndCall:
                boCallsInList = boCallsInList.OrderBy(item => item.timeEndCall);
                break;

            case FiledOfCallInList.volunteerLast:
                boCallsInList = boCallsInList.OrderBy(item => item.volunteerLast);
                break;

            case FiledOfCallInList.TimeEndTreat:
                boCallsInList = boCallsInList.OrderBy(item => item.TimeEndTreat);
                break;

            case FiledOfCallInList.status:
                boCallsInList = boCallsInList.OrderBy(item => item.status);
                break;

            case FiledOfCallInList.numOfAssignments:
                boCallsInList = boCallsInList.OrderBy(item => item.numOfAssignments);
                break;
        }

        return boCallsInList;
    }

    public BO.Call ReadCall(int id)
    {
        var doCall = _dal.Call.Read(c => c.ID == id);

        if (doCall == null)
            throw new BO.BlDoesNotExistException($"Call with ID {id} does not exist in the database.");

        var assignmentsForCall = _dal.Assignment.ReadAll(a => a.CallId == id);

        var boCall = CallsManager.ConvertDOCallWithAssignments(doCall, assignmentsForCall);

        return boCall;
    }

    public void UpdateCall(BO.Call c)
    {

        try
        {
            CallsManager.CheckCallFormat(c);
            CallsManager.CheckCallLogic(c);
            _dal.Call.Update(Helpers.CallsManager.convertFormBOCallToDo(c));
        }
        catch (BO.BlValidationException ex)
        {
            throw new BlValidationException("An error occurred while updating the call.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while updating the call.", ex);
        }
    }



    public void DeleteCall(int id)
    {
        try
        {
            var doCall = _dal.Call.Read(c => c.ID == id);

            if (doCall != null)
            {

                BO.Status callStatus = CallsManager.GetCallStatus(doCall);

                var hasAssignments = _dal.Assignment.ReadAll(ass => ass.CallId == id).Any();

                if (hasAssignments || callStatus != BO.Status.Open)
                {
                    throw new InvalidOperationException($"Cannot delete call with ID {id} as it is not in an open status or has been assigned.");
                }
            }
            _dal.Call.Delete(id);
        }
       catch(DO.DalDoesNotExistException ex)
       {
            throw new BO.BlDoesNotExistException("An error occurred while updating the call.", ex);
       }
    }


    public void CreateCall(BO.Call c)
    {
        try
        {
            CallsManager.CheckCallFormat(c);
            CallsManager.CheckCallLogic(c);

            _dal.Call.Create(Helpers.CallsManager.convertFormBOCallToDo(c));
        }
        catch (BO.BlValidationException ex)
        {
            throw new BlValidationException("An error occurred while updating the call.");
        }
    }

    IEnumerable<ClosedCallInList> ICall.ReadCloseCallsVolunteer(int id, BO.CallType? callT, FiledOfClosedCallInList? filedTosort)
    {
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
       List < BO.ClosedCallInList >  closedCallInLists = new List<BO.ClosedCallInList>();

         closedCallInLists.AddRange(from assig in assignments
                       where assig.finishT != null
                       let listForCall = ReadCall(assig.CallId).CallAssign
                       let assinmetNotTothisvlounteer = listForCall.Count(assForCall => assForCall.VolunteerId != id)
                       where assinmetNotTothisvlounteer == 0
                       select CallsManager.ConvertDOCallToBOCloseCallInList(_dal.Call.Read(c => c.ID == assig.CallId), assig)


            );
        closedCallInLists = closedCallInLists
   .OrderByDescending(closeCall => closeCall.openTime)  // סדר את השיחות לפי openTime
   .DistinctBy(closeCall => closeCall.callT)  // דאג לכך שכל callT יהיה ייחודי
   .ToList();  // המרה לרשימה

        if (callT != null)
        {
            closedCallInLists = closedCallInLists.Where(c => c.callT == callT).ToList();
        }

        if (filedTosort != null)
        {
            switch (filedTosort)
            {
                case BO.FiledOfClosedCallInList.ID:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.ID).ToList();
                    break;
                case BO.FiledOfClosedCallInList.address:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.address).ToList();
                    break;
                case BO.FiledOfClosedCallInList.callT:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.callT).ToList();
                    break;
                case BO.FiledOfClosedCallInList.openTime:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.openTime).ToList();
                    break;
                case BO.FiledOfClosedCallInList.startTreatment:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.startTreatment).ToList();
                    break;
                case BO.FiledOfClosedCallInList.finishTreatment:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.finishTreatment).ToList();
                    break;
                case BO.FiledOfClosedCallInList.finishT:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.finishT).ToList();
                    break;
                case BO.FiledOfClosedCallInList.CallDistance:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.CallDistance).ToList();
                    break;
            }
        }

        return closedCallInLists;
    }

    IEnumerable<OpenCallInList> ICall.ReadOpenCallsVolunteer(int id, BO.CallType? callT, FiledOfOpenCallInList? filedTosort)
    {
      

        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList> Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = ReadCall(item.ID)
                       where DataCall.statusC == BO.Status.Open || DataCall.statusC == BO.Status.OpenInRisk
                       let volunteerData = _dal.Volunteer.Read(v=>v.ID == id)
                       where volunteerData.maxDistance==null ?true : volunteerData.maxDistance<=Tools.CalculateDistance(volunteerData.Latitude?? DataCall.latitude, volunteerData.Longitude?? DataCall.longitude, DataCall.latitude,DataCall.longitude)
                       select CallsManager.ConvertDOCallToBOOpenCallInList(item, id));

        IEnumerable<BO.OpenCallInList> openCallInLists = Calls;

        if (callT != null)
        {
            openCallInLists = openCallInLists.Where(c => c.callT == callT);
        }

        if (filedTosort != null)
        {
            switch (filedTosort)
            {
                case BO.FiledOfOpenCallInList.ID:
                    openCallInLists = openCallInLists.OrderBy(item => item.ID);
                    break;
                case BO.FiledOfOpenCallInList.address:
                    openCallInLists = openCallInLists.OrderBy(item => item.address);
                    break;
                case BO.FiledOfOpenCallInList.callT:
                    openCallInLists = openCallInLists.OrderBy(item => item.callT);
                    break;
                case BO.FiledOfOpenCallInList.openTime:
                    openCallInLists = openCallInLists.OrderBy(item => item.openTime);
                    break;
                case BO.FiledOfOpenCallInList.maxTime:
                    openCallInLists = openCallInLists.OrderBy(item => item.maxTime);
                    break;
                case BO.FiledOfOpenCallInList.verbalDescription:
                    openCallInLists = openCallInLists.OrderBy(item => item.verbalDescription);
                    break;
            }

        }

        return openCallInLists;
    }


    public void FinishTreat(int volunteerId, int assignmentId)
    {
        DO.Assignment assignment;
        try
        {
            assignment = _dal.Assignment.Read(a => a.ID == assignmentId)
                ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
        }

        if (assignment.VolunteerId != volunteerId)
        {
            throw new BO.VolunteerCantUpadeOtherVolunteerException($"Volunteer with ID {volunteerId} is not authorized to finish this assignment.");
        }

        if (assignment.finishTreatment != null || assignment.finishT != null)
        {
            throw new BO.AssignmentAlreadyClosedException($"Assignment with ID {assignmentId} is already closed.");
        }

        assignment = assignment with
        {
            finishTreatment = DateTime.Now,
            finishT = DO.FinishType.Treated
        };

        try
        {
            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"An error occurred while updating the assignment.", ex);
        }
    }


    public void cancelTreat(int volunteerId, int assignmentId)

    {
        DO.Assignment assignment;
        DO.Call call;

        try
        {
            assignment = _dal.Assignment.Read(a => a.ID == assignmentId)
                ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
        }

        try
        {
            call = _dal.Call.Read(c => c.ID == assignment.CallId)
                ?? throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {assignment.CallId} does not exist.", ex);
        }

        if (call.maxTime.HasValue && DateTime.Now > call.maxTime.Value)
        {
            throw new BO.CantUpdatevolunteer($"Call with ID {assignment.CallId} is expired and cannot be canceled.");
        }

        if (!_dal.Volunteer.Read(v => v.ID == volunteerId)?.role.Equals(DO.RoleType.Manager) == true)
        {
            throw new BO.VolunteerCantUpadeOtherVolunteerException($"Volunteer with ID {volunteerId} is not authorized to cancel this assignment.");
        }

        if (assignment.finishTreatment != null || assignment.finishT != null)
        {
            throw new BO.CantUpdatevolunteer($"Assignment with ID {assignmentId} is already closed or cancelled.");
        }

        assignment = assignment with
        {
            finishTreatment = DateTime.Now,
            finishT = (assignment.VolunteerId == volunteerId) ? DO.FinishType.SelfCancel : DO.FinishType.ManagerCancel
        };

        try
        {
            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"An error occurred while updating the assignment.", ex);
        }
    }

    public void ChooseCallTreat(int volunteerId, int callId)
    {
        // Retrieve the call data based on the callId
        var doCall = _dal.Call.Read(c => c.ID == callId);

        // Retrieve all assignments related to the call
        var assignmentsForCall = _dal.Assignment.ReadAll(a => a.CallId == callId);

        // Check if the call has already been treated
        BO.Status callStatus = CallsManager.GetCallStatus(doCall);
        if (callStatus == BO.Status.Close)
        {
            throw new InvalidOperationException("The call has already been treated.");
        }

        // Check if there is an open assignment for the call
        var openAssignment = assignmentsForCall.FirstOrDefault(a => a.startTreatment != default(DateTime) && a.finishT == null);
        if (openAssignment != null)
        {
            throw new InvalidOperationException("The call is already being treated.");
        }

        // Check if the call has expired
        if (callStatus == BO.Status.Expired)
        {
            throw new InvalidOperationException("The call has expired.");
        }

        // Once all checks pass, create a new assignment
        var newAssignment = new DO.Assignment
        {
            CallId = callId,
            VolunteerId = volunteerId,
            startTreatment = DateTime.Now,  // Set the entry time for the treatment
            finishTreatment = null,  // Treatment is not finished yet
            finishT = null  // Treatment type is not specified yet
        };

        // Attempt to add the new assignment to the data layer
        _dal.Assignment.Create(newAssignment);
    }

}