using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Nodes;

namespace Noire.Graphics {
    public interface INode : IDisposable {

        bool UpdateEnabled { get; set; }
        bool RenderEnabled { get; set; }
        void Update();
        void Render();
        INode Parent { get; set; }
        bool IsRoot { get; }
        void AddChild(INode node);
        void RemoveChild(INode node);
        bool IsAncestorOf(INode node);
        ReadOnlyCollection<INode> Children { get; }
        SceneNode Scene { get; }

    }
}
