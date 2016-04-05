using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public interface IUpdateable {

        int UpdateOrder { get; set; }
        bool Enabled { get; set; }
        void Update(GameTime gameTime);
        event EventHandler<EventArgs> UpdateOrderChanged;
        event EventHandler<EventArgs> EnabledChanged;

    }
}
