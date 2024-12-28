using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;



    internal class FiledToFilterVlounteerColecthion : IEnumerable
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

