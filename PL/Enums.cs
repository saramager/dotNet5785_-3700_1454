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

