namespace Noire.Common {
    public interface IGameComponent {

        void Initialize();
        bool IsInitialized { get; }

    }
}
