using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc.Extensions {
    public static class EnumerableExtensions {

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            if (enumerable == null) {
                return;
            }
            foreach (var element in enumerable) {
                action.Invoke(element);
            }
        }

        public static void ForEach(this IEnumerable enumerable, Action<object> action) {
            if (enumerable == null) {
                return;
            }
            foreach (var element in enumerable) {
                action(element);
            }
        }

    }
}
