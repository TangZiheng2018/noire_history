using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public sealed class GameComponentCollection : Collection<GameComponent> {

        protected override void InsertItem(int index, GameComponent item) {
            // 处理 UpdateOrder 和 DrawOrder 冲突
            if (Count > 0) {
                var updateLarger = from t in this
                                   where t.UpdateOrder >= index
                                   orderby t.UpdateOrder ascending
                                   select t;
                var drawLarger = from t in this
                                 where t.DrawOrder >= index
                                 orderby t.DrawOrder ascending
                                 select t;
                int counter;
                var value = 0;
                var toAdd = 0;
                if (updateLarger.Any()) {
                    counter = 0;
                    foreach (var component in updateLarger) {
                        if (counter == 0) {
                            value = component.UpdateOrder;
                            toAdd = value > index ? 0 : 1;
                        }
                        component.UpdateOrder += value - index + toAdd;
                        ++counter;
                    }
                }
                if (drawLarger.Any()) {
                    counter = 0;
                    foreach (var component in drawLarger) {
                        if (counter == 0) {
                            value = component.DrawOrder;
                            toAdd = value > index ? 0 : 1;
                        }
                        component.DrawOrder += value - index + toAdd;
                        ++counter;
                    }
                }
            }
            base.InsertItem(index, item);
            item.UpdateOrderChanged += OnComponentLoopPropChanged;
            item.DrawOrderChanged += OnComponentLoopPropChanged;
            item.DrawOrder = index;
            item.UpdateOrder = index;
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
