
namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="ID">A number that uniquely identifies the allocation entity.>
/// <param name="CallId">A number that identifies the call that the volunteer chose to handle>
/// <param name="VolunteerId">ID number of the volunteer who chose to take care of the reading>
/// <param name="startTreatment">Time (date and time) when the current call was processed.>
/// <param name="finishTreatment">Time (date and time) when the current volunteer finished handling the current call.>
/// <param name="finishT">The manner in which the treatment of the current reading was completed by the current volunteer.>
public record Assignment
(
  int ID,
  int CallId,
  int VolunteerId,
  DateTime startTreatment,
  DateTime? finishTreatment = null,
  FinishType? finishT = null
 )
{
    public Assignment() : this(0, 0, 0, default(DateTime)) { } // empty ctor for stage 3 
}
