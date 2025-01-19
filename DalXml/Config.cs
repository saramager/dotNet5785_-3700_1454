using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal static class Config
    {
        internal const string s_data_config_xml = "data-config.xml"; // this????
       // internal const string s_data_config = "data-config";    Or this??

        internal const string s_Volunteers_xml = "Volunteers.xml";
        internal const string s_Calls_xml = "Calls.xml";
        internal const string s_Assignments_xml = "Assignments.xml";

        internal static int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
        }

        internal static int NextAssignmentId
        {

            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");

            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
        }

        internal static DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }
        
        internal static TimeSpan RiskRange  //You need to check how to apply in XMLTools
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void Reset()
        {
            NextCallId = 1000;
            NextAssignmentId = 1000;
            Clock = DateTime.Now;
        }

    }
}
