namespace PL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class FiledToFilterVolunteerColecthion : IEnumerable
{
    static readonly IEnumerable<BO.FiledOfVolunteerInList> s_enums =
(Enum.GetValues(typeof(BO.FiledOfVolunteerInList)) as IEnumerable<BO.FiledOfVolunteerInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class VolunteerRolesColecthion : IEnumerable
{
    static readonly IEnumerable<BO.RoleType> s_enums =
(Enum.GetValues(typeof(BO.RoleType)) as IEnumerable<BO.RoleType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class DistanceTypeColecthion : IEnumerable
{
    static readonly IEnumerable<BO.Distance> s_enums =
(Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class callTypeColecthion : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
(Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class statusCColecthion : IEnumerable
{
    static readonly IEnumerable<BO.Status> s_enums =
(Enum.GetValues(typeof(BO.Status)) as IEnumerable<BO.Status>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class FiledOFCloseListColecthion : IEnumerable
{
    static readonly IEnumerable<BO.FiledOfClosedCallInList> s_enums =
(Enum.GetValues(typeof(BO.FiledOfClosedCallInList)) as IEnumerable<BO.FiledOfClosedCallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class FiledOfOpenListColecthion : IEnumerable
{
    static readonly IEnumerable<BO.FiledOfOpenCallInList> s_enums =
(Enum.GetValues(typeof(BO.FiledOfOpenCallInList)) as IEnumerable<BO.FiledOfOpenCallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

