﻿using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class Bl : IBI
    {
       
        public IVolunteer Volunteer { get; } =  new VolunteerImplementation();

        public ICall Call { get; } = new CallImplementation();

        public IAdmin Admin { get; } = new AdminImplementation();
    }
}
