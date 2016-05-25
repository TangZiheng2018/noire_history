using System;

namespace Noire.Common {
    public interface IGameComponentContainer : IGameComponent {

        GameComponentCollection ChildComponents { get; }
        IGameComponent GetChildByName(string name);
        IGameComponent GetChildByType(Type type);
        T GetChildByType<T>() where T : class, IGameComponent;

    }
}
