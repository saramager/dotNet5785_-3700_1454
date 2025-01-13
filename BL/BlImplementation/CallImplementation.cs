namespace BlImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using BO;
using DO;
using Helpers;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddObserver(Action listObserver) =>
CallsManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
CallsManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
CallsManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
CallsManager.Observers.RemoveObserver(id, observer); //stage 5

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


    public IEnumerable<CallInList> GetCallInList(FiledOfCallInList? filedToFilter, object? filter, FiledOfCallInList? filedToSort)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");
        IEnumerable<CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallsManager.ConvertDOCallToBOCallInList(call));

        if (filedToFilter != null && filter != null)
        {
            switch (filedToFilter)
            {
                case FiledOfCallInList.ID:
                    if (filter is string filterStr && int.TryParse(filterStr, out int idValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.ID == idValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for ID filter");
                    }
                    break;

                case FiledOfCallInList.CallId:
                    if (filter is string filterStrCallId && int.TryParse(filterStrCallId, out int callIdValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.CallId == callIdValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for CallId filter");
                    }
                    break;

                case FiledOfCallInList.callT:
                    if (filter is string filterStrCallType && Enum.TryParse<BO.CallType>(filterStrCallType, out BO.CallType callTypeValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.callT == callTypeValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for callT filter");
                    }
                    break;

                case FiledOfCallInList.openTime:
                    if (filter is string filterStrDate && DateTime.TryParse(filterStrDate, out DateTime openTimeValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.openTime == openTimeValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for openTime filter");
                    }
                    break;

                case FiledOfCallInList.timeEndCall:
                    if (filter is string filterStrTimeSpan && TimeSpan.TryParse(filterStrTimeSpan, out TimeSpan timeEndCallValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.timeEndCall == timeEndCallValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for timeEndCall filter");
                    }
                    break;

                case FiledOfCallInList.volunteerLast:
                    if (filter is string filterStrVolunteerLast)
                    {
                        boCallsInList = boCallsInList.Where(item => item.volunteerLast == filterStrVolunteerLast);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for volunteerLast filter");
                    }
                    break;

                case FiledOfCallInList.TimeEndTreat:
                    if (filter is string filterStrTreatTime && TimeSpan.TryParse(filterStrTreatTime, out TimeSpan timeEndTreatValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.TimeEndTreat == timeEndTreatValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for TimeEndTreat filter");
                    }
                    break;

                case FiledOfCallInList.status:
                    if (filter is string filterStrStatus && Enum.TryParse<Status>(filterStrStatus, out Status statusValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.status == statusValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for status filter");
                    }
                    break;

                case FiledOfCallInList.numOfAssignments:
                    if (filter is string filterStrAssignments && int.TryParse(filterStrAssignments, out int numAssignmentsValue))
                    {
                        boCallsInList = boCallsInList.Where(item => item.numOfAssignments == numAssignmentsValue);
                    }
                    else
                    {
                        throw new cantFilterCallException("Invalid value for numOfAssignments filter");
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid filter field");
            }
        }

        // סינון לפי שדה למיון
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
            CallsManager.Observers.NotifyItemUpdated(Helpers.CallsManager.convertFormBOCallToDo(c).ID);  //stage 5
            CallsManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (BO.BlUpdateCallException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while updating the call:", ex);
        }
        catch (BO.BlValidationException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while updating the call:", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while updating the call:", ex);
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
                    throw new CantDeleteCallException($"Cannot delete call with ID {id} as it is not in an open status or has been assigned.");
                }
            }
            _dal.Call.Delete(id);
            CallsManager.Observers.NotifyItemUpdated(id);
            CallsManager.Observers.NotifyListUpdated();  //stage 5  	

        }
        catch (DO.DalDoesNotExistException ex)
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
            CallsManager.Observers.NotifyItemUpdated(c.ID);
            CallsManager.Observers.NotifyListUpdated(); //stage 5               
        }
        catch (BO.BlValidationException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while creating the call:.", ex);
        }
        catch (BO.BlUpdateCallException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while creating the call:", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while creating the call:", ex);
        }
    }

    IEnumerable<ClosedCallInList> BlApi.ICall.ReadCloseCallsVolunteer(int id, BO.CallType? callT, FiledOfClosedCallInList? filedTosort)
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

    IEnumerable<OpenCallInList> BlApi.ICall.ReadOpenCallsVolunteer(int id, BO.CallType? callT, FiledOfOpenCallInList? filedTosort)
    {
      

        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList> Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = ReadCall(item.ID)
                       where DataCall.statusC == BO.Status.Open || DataCall.statusC == BO.Status.OpenInRisk
                       let volunteerData = _dal.Volunteer.Read(v=>v.ID == id)
                       where volunteerData.maxDistance==null ?true : volunteerData.maxDistance>=Tools.CalculateDistance(volunteerData.Latitude?? DataCall.latitude, volunteerData.Longitude?? DataCall.longitude, DataCall.latitude,DataCall.longitude,(BO.Distance)volunteerData.distanceType)
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
            VolunteersManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteersManager.Observers.NotifyListUpdated();
            CallsManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
            CallsManager.Observers.NotifyListUpdated();  //stage 5
            // משקיפים?????
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"An error occurred while updating the assignment.", ex);
        }
    }


    public void cancelTreat(int volunteerId, int? assignmentId)

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

        if (!_dal.Volunteer.Read(v => v.ID == volunteerId)?.role.Equals(DO.RoleType.Manager) == true &&
    _dal.Volunteer.Read(v => v.ID == volunteerId)?.ID != assignment.VolunteerId)
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
            VolunteersManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteersManager.Observers.NotifyListUpdated();
            CallsManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
            CallsManager.Observers.NotifyListUpdated();  //stage 5

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
        VolunteersManager.Observers.NotifyItemUpdated(volunteerId);
        VolunteersManager.Observers.NotifyListUpdated();
        CallsManager.Observers.NotifyItemUpdated(newAssignment.CallId);  //stage 5
        CallsManager.Observers.NotifyListUpdated();  //stage 5
    }

}