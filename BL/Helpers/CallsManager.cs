﻿using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static  class CallsManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
    }
}
