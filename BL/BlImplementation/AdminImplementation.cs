using BlApi;
using BO;
using DalTest;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;

    public void AddToClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        switch (unit)
        {
            case TimeUnit.Minute:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case TimeUnit.Hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case TimeUnit.Day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case TimeUnit.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case TimeUnit.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
        }
     }

    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }

    public void RestartDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7

    }

    public void SetRiskRange(TimeSpan range)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        AdminManager.RiskRange = range;
    }

    public void InitializeDB()
    {

        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7


    }
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

}
