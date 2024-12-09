using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class VolunteerInList //init or set??????
    {
       public int ID { get; init; }
       public string fullName { get; init; }
        public bool active { get; init; }
        public int numCallsHandled { get; init; }
        public int numCallsCancelled { get; init; }
        public int numCallsExpired { get; init; }
       public int? CallId { get; init; }
        public CallType callT { get; init; }

         public override string ToString() => this.ToStringProperty(); // in 7ג
    }


}

