using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;

public interface IAdmin
{
    DateTime GetClock();
    void AddToClock(TimeUnit unit);
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan range);
    void RestartDB();
    void InitializeDB();
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7


}
