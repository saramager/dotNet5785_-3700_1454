using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OpenCallInList//init or set??????
    {
        public int ID { get; init; }
        public string address { get; init; }
        public CallType callT { get; init; }
        public DateTime openTime { get; init; }
        public DateTime? maxTime { get; init; }
        public string? verbalDescription { get; init; }
        public double distance { get; init; }   

        public override string ToString() => this.ToStringProperty();// in 7ג
    }
}
