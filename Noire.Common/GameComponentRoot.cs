namespace Noire.Common {
    public abstract class GameComponentRoot : GameComponentContainer, IGameComponentRoot {

        protected GameComponentRoot()
            : base(null, null) {
            _parentContainer = _rootContainer = this;
        }

    }
}
