using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public interface IDrawable {

        int DrawOrder { get; set; }
        bool Visible { get; set; }
        void Draw(GameTime gameTime);
        event EventHandler<EventArgs> DrawOrderChanged;
        event EventHandler<EventArgs> VisibleChanged;

    }
}
