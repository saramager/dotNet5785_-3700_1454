using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using BO;
using DalApi;
using Helpers;
namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
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
                    boCallsInList = boCallsInList.Where(item => item.callT == (CallType)sort);
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

    void UpdateCall(Call c)
    {
        throw new NotImplementedException();
    }

   
   
   
  
 
    void BlApi.ICall.UpdateCall(Call c)
    {
        throw new NotImplementedException();
    }

    void BlApi.ICall.DeleteCall(int id)
    {
        var call = ReadCall(id);
        if (!(call.statusC == BO.Status.Open && call.CallAssign == null))
            throw new CantDeleteCallException($"can delete this call id: {id}");
        try
        {
            _dal.Call.Delete(id);
        }
        catch (DO.DalDoesNotExistException dEx) { throw new BO.BlDoesNotExistException(dEx.Message, dEx); }
    }

    public void CreateCall(Call c)
    {
        throw new NotImplementedException();
    }

    IEnumerable<ClosedCallInList> BlApi.ICall.ReadCloseCallsVolunteer(int id, CallType? callT, FiledOfClosedCallInList? filedTosort)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.ClosedCallInList> Calls = new List<BO.ClosedCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = ReadCall(item.ID)
                       where DataCall.statusC == BO.Status.Close && DataCall.CallAssign?/*.AssignmentsToCalls?*/.Any() == true
                       let lastAssugnment = DataCall.CallAssign.OrderBy(c => c.startTreatment).Last()
                       select CallsManager.ConvertDOCallToBOCloseCallInList(item, lastAssugnment));

        IEnumerable<BO.ClosedCallInList> closedCallInLists = Calls.Where(call => call.ID == id);

        if (callT != null)
        {
            closedCallInLists = closedCallInLists.Where(c => c.callT == callT);
        }

        if (filedTosort != null)
        {
            switch (filedTosort)
            {
                case BO.FiledOfClosedCallInList.ID:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.ID);
                    break;
                case BO.FiledOfClosedCallInList.address:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.address);
                    break;
                case BO.FiledOfClosedCallInList.callT:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.callT);
                    break;
                case BO.FiledOfClosedCallInList.openTime:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.openTime);
                    break;
                case BO.FiledOfClosedCallInList.startTreatment:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.startTreatment);
                    break;
                case BO.FiledOfClosedCallInList.finishTreatment:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.finishTreatment);
                    break;
                case BO.FiledOfClosedCallInList.finishT:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.finishT);
                    break;
                case BO.FiledOfClosedCallInList.CallDistance:
                    closedCallInLists = closedCallInLists.OrderBy(item => item.CallDistance);
                    break;
            }
        }

        return closedCallInLists;
    }

    IEnumerable<OpenCallInList> BlApi.ICall.ReadOpenCallsVolunteer(int id, CallType? callT, FiledOfOpenCallInList? filedTosort)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList> Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = ReadCall(item.ID)
                       where DataCall.statusC == BO.Status.Open || DataCall.statusC == BO.Status.OpenInRisk
                       let lastAssugnment = DataCall.CallAssign.OrderBy(c => c.startTreatment).Last()
                       select CallsManager.ConvertDOCallToBOOpenCallInList(item, id));

        IEnumerable<BO.OpenCallInList> openCallInLists = Calls.Where(call => call.ID == id);

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
 

    void BlApi.ICall.FinishTertment(int Vid, int AssignmentId)
    {
        throw new NotImplementedException();
    }

    void BlApi.ICall.ToTreat(int Vid, int CId)
    {
        throw new NotImplementedException();
    }
}
