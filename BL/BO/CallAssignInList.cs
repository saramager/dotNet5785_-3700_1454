using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallAssignInList //init or set??????
    {
        public int VolunteerId { get; init; }
        public string fullName { get; init; }
        public DateTime startTreatment { get; init; }
        public DateTime? finishTreatment { get; init; }
        public FinishType? finishT { get; init; }

         public override string ToString() => this.ToStringProperty();// in 7ג
    }
}
