using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public abstract class GameComponentContainer : GameComponent, IGameComponentContainer {

        protected GameComponentContainer() {
            _childComponents = new GameComponentCollection();
            _childComponents.ComponentAdded += OnChildComponentsChanged;
            _childComponents.ComponentRemoved += OnChildComponentsChanged;
            _childComponents.ComponentLoopPropChanged += OnChildComponentLoopPropChanged;
            _drawList = new List<GameComponent>();
            _updateList = new List<GameComponent>();
            _childrenChanged = false;
        }

        public GameComponentCollection ChildComponents => _childComponents;

        protected override void UpdateInternal(GameTime gameTime) {
            if (_childrenChanged) {
                UpdateDrawListAndUpdateList();
            }
            foreach (var item in _updateList) {
                item.Update(gameTime);
            }
        }

        protected override void DrawInternal(GameTime gameTime) {
            foreach (var item in _drawList) {
                item.Draw(gameTime);
            }
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            _childComponents.ComponentAdded -= OnChildComponentsChanged;
            _childComponents.ComponentRemoved -= OnChildComponentsChanged;
            _childComponents.ComponentLoopPropChanged -= OnChildComponentLoopPropChanged;
            if (_childComponents.Count > 0) {
                foreach (var child in _childComponents) {
                    child.Dispose();
                }
                _childComponents.Clear();
            }
            _drawList.Clear();
            _updateList.Clear();
            _drawList = null;
            _updateList = null;
            _childComponents = null;
            base.Dispose(disposing);
        }

        protected internal override void RaiseSurfaceInvalidated(object sender, EventArgs e) {
            base.RaiseSurfaceInvalidated(sender, e);
            foreach (var item in _childComponents) {
                item.RaiseSurfaceInvalidated(sender, e);
            }
        }

        private void OnChildComponentsChanged(object sender, GameComponentCollectionEventArgs e) {
            _childrenChanged = true;
        }

        private void OnChildComponentLoopPropChanged(object sender, EventArgs e) {
            _childrenChanged = true;
        }

        private void UpdateDrawListAndUpdateList() {
            _drawList.Clear();
            _updateList.Clear();
            _drawList.AddRange(_childComponents);
            _updateList.AddRange(_childComponents);
            _updateList.Sort((c1, c2) => c1.UpdateOrder - c2.UpdateOrder);
            _drawList.Sort((c1, c2) => c1.DrawOrder - c2.DrawOrder);
            _childrenChanged = false;
        }

        private GameComponentCollection _childComponents;
        private List<GameComponent> _drawList;
        private List<GameComponent> _updateList;
        private bool _childrenChanged;

    }
}
