using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ClosedCallInList //init or set??????
    {
        public int ID { get; init; }
        public string address { get; init; }
        public CallType callT { get; init; }
        public DateTime openTime { get; init; }
        public DateTime startTreatment { get; init; }
        public DateTime? finishTreatment { get; init; }
        public FinishType? finishT { get; init; }
        public double CallDistance { get; init; }

        public override string ToString() => this.ToStringProperty();
    }
}
