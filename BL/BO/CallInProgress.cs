using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallInProgress // init or set??????
    {
        public int ID { get; init; }
        public int CallId { get; init; }
        public CallType callT { get; init; }
        public string? verbalDescription { get; init; }
        public string address { get; init; }
        public DateTime openTime { get; init; }
        public DateTime? maxTime { get; init; }
        public DateTime startTreatment { get; init; }
        public double CallDistance { get; init; }
         public Status statusT { get; init; }

         public override string ToString() => this.ToStringProperty();// in 7ג

    }
}
