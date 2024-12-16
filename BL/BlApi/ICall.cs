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
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    int[] SumOfCalls();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filedToFilter"></param>
    /// <param name="sort"></param>
    /// <param name="filedToSort"></param>
    /// <returns></returns>
    IEnumerable< CallInList> GetCallInList (FiledOfCallInList? filedToFilter, object? sort , FiledOfCallInList? filedToSort) ;
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Call ReadCall(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
   void  UpdateCall(Call c);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    void DeleteCall(int id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    void CreateCall (Call c);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callT"></param>
    /// <param name="filedTosort"></param>
    /// <returns></returns>
    IEnumerable<ClosedCallInList> ReadCloseCallsVolunteer(int id, CallType? callT, FiledOfClosedCallInList? filedTosort);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callT"></param>
    /// <param name="filedTosort"></param>
    /// <returns></returns>
    IEnumerable<OpenCallInList> ReadOpenCallsVolunteer(int id, CallType? callT, FiledOfOpenCallInList? filedTosort);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Vid"></param>
    /// <param name="AssignmentId"></param>
     void FinishTertment(int Vid,int AssignmentId );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Vid"></param>
    /// <param name="CId"></param>
    void ToTreat(int Vid, int CId);

}
