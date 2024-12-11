using BlApi;
using BO;
using DalTest;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddToClock(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Minute:
                    ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                    break;
                case TimeUnit.Hour:
                    ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                    break;
                case TimeUnit.Day:
                    ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                    break;
                case TimeUnit.Month:
                    ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
                    break;
                case TimeUnit.Year:
                    ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
                    break;
            }
         }

        public DateTime GetClock()
        {
            return ClockManager.Now;
        }

        public TimeSpan GetRiskRange()
        {
            return _dal.Config.RiskRange;
        }

        public void RestartDB()
        {
           _dal.ResetDB();  
        }

        public void SetRiskRange(TimeSpan range)
        {

           _dal.Config.RiskRange = range;
        }

        public void UpdateDB()
        {
          
           Initialization.Do();//do RESET DB and INSTATion 
           


        }
    }
}
