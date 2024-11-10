// Module Call.cs
using System.Net;

namespace DO;



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
