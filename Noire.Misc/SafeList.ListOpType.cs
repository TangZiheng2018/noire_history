using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc
{
    public partial class SafeList<T>
    {

        private enum ListOpType
        {
			Add,
            AddRange,
			Insert,
            InsertRange,
			Remove,
            RemoveAt,
            RemoveRange,
            Clear
        }

    }
}
