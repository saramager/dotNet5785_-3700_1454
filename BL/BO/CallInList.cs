using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BO
{
    public class CallInList // init or set??????
    {
        public int? ID { get; init; }
        public int CallId { get; init; }
        public CallType callT { get; init; }
        public DateTime openTime { get; init; }
        public TimeSpan? timeEndCall { get; init; }
        public string? volunteerLast { get; init; }
        public TimeSpan? TimeEndTreat { get; init; }
        public Status status { get; init; }
        public int numOfAssignments { get; init; }

        public override string ToString() => this.ToStringProperty();// in 7ג
    }

}
