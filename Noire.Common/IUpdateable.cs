using System;

namespace Noire.Common {
    public interface IUpdateable {

        int UpdateOrder { get; set; }
        bool Enabled { get; set; }
        void Update(GameTime gameTime);
        event EventHandler<EventArgs> UpdateOrderChanged;
        event EventHandler<EventArgs> EnabledChanged;

    }
}
