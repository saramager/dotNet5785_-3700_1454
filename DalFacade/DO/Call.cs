
namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="ID">A number that uniquely identifies the call.>
/// <param name="address">Full and real address in correct format, of the reading location>
/// <param name="callT">according to the specific system type>
/// <param name="latitude">A number indicating how far a point on Earth is south or north of the equator.>
/// <param name="longitude">A number indicating how far a point on Earth is east or west of the equator.>
/// <param name="openTime">Time (date and time) when the call was opened by the manager.>
/// <param name="maxTime">Time (date and time) by which the reading should be closed.>
/// <param name="verbalDescription">Description of the call>

public record Call
(

    int ID,
    string address,
    CallType callT,
    double? latitude,
    double? longitude,
    DateTime openTime,
    DateTime? maxTime = null,
    string? verbalDescription = null

)

{
    public Call() : this(0, "", default, 0, 0, default(DateTime)) { } // empty ctor for stage 3 
}