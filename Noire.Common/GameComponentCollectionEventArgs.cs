using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public sealed class GameComponentCollectionEventArgs : EventArgs {

        public GameComponentCollectionEventArgs(GameComponent item) {
            _item = item;
        }

        public GameComponent Item => _item;

        private readonly GameComponent _item;

    }
}
