using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.VariablePipeline {
    public static class Range {

        public static IEnumerable<int> Int32(int start, int count) {
            for (var i = start; i < start + count; ++i) {
                yield return i;
            }
        }

    }
}
