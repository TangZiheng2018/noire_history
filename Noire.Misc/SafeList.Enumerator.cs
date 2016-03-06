using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc {
    public partial class SafeList<T> {

        public sealed class Enumerator : IEnumerator<T> {

            internal Enumerator(SafeList<T> list) {
                _list = list;
                Reset();
            }

            public T Current {
                get {
                    if (_index < 0 || _index >= _list.Count) {
                        throw new InvalidOperationException("This enumerator is not pointing to a valid value.");
                    }
                    return _list[_index];
                }
            }

            object IEnumerator.Current {
                get {
                    return Current;
                }
            }

            public void Dispose() {
                _list.OnEnumeratorDisposed(this);
            }

            public bool MoveNext() {
                if (_index >= _list.Count - 1) {
                    return false;
                } else {
                    ++_index;
                    return true;
                }
            }

            public void Reset() {
                _index = -1;
            }

            private int _index;

            private SafeList<T> _list;

        }

    }
}
