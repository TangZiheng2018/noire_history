using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public sealed class GameComponentCollection : Collection<GameComponent> {

        protected override void InsertItem(int index, GameComponent item) {
            base.InsertItem(index, item);
            item.UpdateOrderChanged += OnComponentLoopPropChanged;
            item.DrawOrderChanged += OnComponentLoopPropChanged;
            item.DrawOrder = index;
            item.UpdateOrder = index;
            // TODO: 然后呢？如何确定新的更新和绘制顺序？
            NoireUtilities.RaiseEvent(ComponentAdded, this, new GameComponentCollectionEventArgs(item));
        }

        protected override void RemoveItem(int index) {
            var item = this[index];
            base.RemoveItem(index);
            item.UpdateOrderChanged -= OnComponentLoopPropChanged;
            item.DrawOrderChanged -= OnComponentLoopPropChanged;
            NoireUtilities.RaiseEvent(ComponentRemoved, this, new GameComponentCollectionEventArgs(item));
        }

        private void OnComponentLoopPropChanged(object sender, EventArgs e) {
            NoireUtilities.RaiseEvent(ComponentLoopPropChanged, sender, e);
        }

        public event EventHandler<GameComponentCollectionEventArgs> ComponentAdded;
        public event EventHandler<GameComponentCollectionEventArgs> ComponentRemoved;
        public event EventHandler<EventArgs> ComponentLoopPropChanged;

    }
}
