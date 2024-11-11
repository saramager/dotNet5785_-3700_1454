// Module Call.cs
using System.Net;

namespace DO;


/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <param name="address"></param>
/// <param name="latitude"></param>
/// <param name="longitude"></param>
/// <param name="openTime"></param>
/// <param name="maxTime"></param>
/// <param name="verbalDescription"></param>
public record Call
(

    int id,
    string  address,
    // סוג הקריאה  ,
    double latitude,
    double longitude,
    DateTime openTime,
    DateTime? maxTime = null,
    string? verbalDescription = null

)
 
{
    public Call() : this(0, "",0,0, default(DateTime)) { } // empty ctor for stage 3 
}
