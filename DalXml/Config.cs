using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal static class Config
    {
        internal const string s_data_config_xml = "data-config.xml"; // this????
       // internal const string s_data_config = "data-config";    Or this??

        internal const string s_Volunteers_xml = "Volunteers_xml";
        internal const string s_Calls_xml = "Calls_xml";
        internal const string s_Assignments_xml = "Assignments_xml";

        internal static int NextCallId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
        }

        internal static int NextAssignmentId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
        }

        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }
        
        internal static TimeSpan RiskRange  //You need to check how to apply in XMLTools
        {
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }

        internal static void Reset()
        {
            NextCallId = 1000;
            NextAssignmentId = 1000;
            Clock = DateTime.Now;
        }

    }
}
