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
    void UpdateDB();
}
