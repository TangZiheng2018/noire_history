using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc.Extensions {
    public static class EventExtensions {

        public static void RaiseEvent<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs {
            handler?.Invoke(sender, e);
        }

        public static void RaiseEvent(this EventHandler handler, object sender, EventArgs e) {
            handler?.Invoke(sender, e);
        }

    }
}
