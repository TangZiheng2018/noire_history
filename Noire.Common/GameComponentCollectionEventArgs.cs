using System;

namespace Noire.Common {
    public sealed class GameComponentCollectionEventArgs : EventArgs {

        public GameComponentCollectionEventArgs(GameComponent item) {
            _item = item;
        }

        public GameComponent Item => _item;

        private readonly GameComponent _item;

    }
}
