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

        public static IEnumerable<int> ReversedInt32(int end, int count) {
            for (var i = end - 1; i >= 0; --i) {
                yield return i;
            }
        }

    }
}
