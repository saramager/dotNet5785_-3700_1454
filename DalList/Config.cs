
using System.Runtime.CompilerServices;

namespace Dal;


 internal static class Config
{
    internal const int StartCallId = 1000;

    private static int NextCallId = StartCallId;
    internal static int nextCallId { [MethodImpl(MethodImplOptions.Synchronized)] get => NextCallId++; }


    internal const int StartAssignmentId = 1000;

    private static int NextAssignmentId = StartAssignmentId;
    internal static int nextAssignmentId { [MethodImpl(MethodImplOptions.Synchronized)] get => NextAssignmentId++; }

    internal static DateTime Clock { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; } = DateTime.Now;
    internal static TimeSpan RiskRange { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextCallId = StartCallId;
        NextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
    }





}
