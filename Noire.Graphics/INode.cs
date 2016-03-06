using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Graphics {
    public interface INode : IDisposable {

        bool Enabled { get; set; }
        bool Visible { get; set; }
        void Update();
        void Render();
        INode Parent { get; set; }
        bool IsRoot { get; }
        void AddChild(INode node);
        void RemoveChild(INode node);
        ReadOnlyCollection<INode> Children { get; }
        Direct3DRuntime D3DRuntime { get; }

    }
}
