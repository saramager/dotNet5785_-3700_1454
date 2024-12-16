using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Volunteer
    {
        public int Id { get; init; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string? currentAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public RoleType role { get; set; }
        public bool active { get; set; }
        public double? maxDistance { get; set; }
        public Distance distanceType { get; set; }
        public int numCallsHandled { get; set; }
        public int numCallsCancelled { get; set; }
        public int numCallsExpired { get; set; }
        public BO.CallInProgress? callProgress { get; set; }

        public override string ToString() => this.ToStringProperty();


    }
}
