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
public interface ICall
{
    int[] SumOfCalls();
    IEnumerable< CallInList> GetCallInList (FiledOfCallInList? filedToFilter, object? sort , FiledOfCallInList? filedToSort) ;

    Call ReadCall(int id);

   void  UpdateCall(Call c);
    void DeleteCall(int id);
    void CreateCall (Call c);
    IEnumerable<ClosedCallInList> ReadCloseCallsVolunteer(int id, CallType? callT, FiledOfClosedCallInList? filedTosort);

    IEnumerable<OpenCallInList> ReadOpenCallsVolunteer(int id, CallType? callT, FiledOfOpenCallInList? filedTosort);

     void FinishTertment(int Vid,int AssignmentId );
    void ToTreat(int Vid, int CId);

}
