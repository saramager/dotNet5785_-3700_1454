using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   
    public class Call
    {
        public int ID { get; init; }
        public string address { get; set; }
        public CallType callT { get; set; }
        public string? verbalDescription { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DateTime openTime { get; set; }
        public DateTime? maxTime { get; set; }
        public Status statusC { get; set; }
        List<BO.CallAssignInList>? CallAssign { get; set; }

         public override string ToString() => this.ToStringProperty();// in 7ג
    }
}
