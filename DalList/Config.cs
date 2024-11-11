
namespace Dal;


 internal static class Config
{
    internal const int StartCallId = 1000;

    private static int NextCallId = StartCallId;
    internal static int nextCallId { get => NextCallId++; }


    internal const int StartAssignmentId = 1000;

    private static int NextAssignmentId = StartAssignmentId;
    internal static int nextAssignmentId { get => NextAssignmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; }

    internal static void Reset()
    {
        NextCallId = StartCallId;
        NextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
    }





}
