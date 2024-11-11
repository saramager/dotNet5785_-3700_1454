
namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="ID"></param>
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="startTreatment"></param>
/// <param name="finishTreatment"></param>
/// <param name="typeFinish"></param>
public record Assignment
(
  int ID,
  int CallId,
  int VolunteerId,
  DateTime startTreatment,
  DateTime? finishTreatment = null,
  Enum? typeFinish = null
 )
{
    public Assignment() : this(0, 0, 0, default(DateTime)) { } // empty ctor for stage 3 
}

}
