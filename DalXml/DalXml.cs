using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    /// <summary>
    ///Implementation of IDal based on XML:
    ///DalXml is responsible for accessing data stored in XML files
    /// </summary>
    sealed public class DalXml : IDal
    {
        public IAssignment Assignment { get; } = new AssignmentImplementation();

        public ICall Call { get; } = new CallImplementation();

        public IVolunteer Volunteer { get; } = new VolunteerImplementation();

        public IConfig Config { get; } = new ConfigImplementation();

        public void ResetDB()
        {
            Assignment.DeleteAll();
            Call.DeleteAll();
            Volunteer.DeleteAll();
            Config.Reset();
        }
    }
}
