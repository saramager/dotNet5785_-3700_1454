﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using BO;
using Helpers;
namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
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

    void ICall.CreateCall(Call c)
    {
        throw new NotImplementedException();
    }

    void ICall.DeleteCall(int id)
    {
        throw new NotImplementedException();
    }

    void ICall.FinishTertment(int Vid, int AssignmentId)
    {
        throw new NotImplementedException();
    }

    IEnumerable<CallInList> ICall.GetCallInList(FiledOfCallInList? filedToFilter, object? sort, FiledOfCallInList? filedToSort)
    {
        throw new NotImplementedException();
    }

    Call ICall.ReadCall(int id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<ClosedCallInList> ICall.ReadCloseCallsVolunteer(int id, CallType? callT, FiledOfClosedCallInList? filedTosort)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.ClosedCallInList> Calls = new List<BO.ClosedCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.Status.Close && DataCall.AssignmentsToCalls?.Any() == true
                       let lastAssugnment = DataCall.AssignmentsToCalls.OrderBy(c => c.StartTreat).Last()
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
  

    IEnumerable<OpenCallInList> ICall.ReadOpenCallsVolunteer(int id, CallType? callT, FiledOfOpenCallInList? filedTosort)
    {
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList> Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.Status.Open || DataCall.Status == BO.Status.OpenInRisk
                       let lastAssugnment = DataCall.AssignmentsToCalls.OrderBy(c => c.StartTreat).Last()
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

    int[] ICall.SumOfCalls()
    {
        throw new NotImplementedException();
    }

    void ICall.ToTreat(int Vid, int CId)
    {
        throw new NotImplementedException();
    }

    void ICall.UpdateCall(Call c)
    {
        throw new NotImplementedException();
    }
}
