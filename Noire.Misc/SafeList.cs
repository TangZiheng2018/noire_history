using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc {
    public sealed partial class SafeList<T> : IList<T> {

        public SafeList() {
            _list = new List<T>();
        }

        public SafeList(int capacity) {
            _list = new List<T>(capacity);
        }

        public SafeList(IEnumerable<T> collection) {
            _list = new List<T>(collection);
        }

        public ReadOnlyCollection<T> AsReadOnly() => _list.AsReadOnly();

        public int BinarySearch(T item) => _list.BinarySearch(item);

        public int BinarySearch(T item, IComparer<T> comparer) => _list.BinarySearch(item, comparer);

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => _list.BinarySearch(index, count, item, comparer);

        public int Capacity => _list.Capacity;

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => _list.ConvertAll(converter);

        public bool Exists(Predicate<T> match) => _list.Exists(match);

        public T Find(Predicate<T> match) => _list.Find(match);

        public List<T> FindAll(Predicate<T> match) => _list.FindAll(match);

        public int FindIndex(Predicate<T> match) => _list.FindIndex(match);

        public int FindIndex(int startIndex, Predicate<T> match) => _list.FindIndex(startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match) => _list.FindIndex(startIndex, count, match);

        public int FindLastIndex(Predicate<T> match) => _list.FindLastIndex(match);

        public int FindLastIndex(int startIndex, Predicate<T> match) => _list.FindLastIndex(startIndex, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match) => _list.FindLastIndex(startIndex, count, match);

        public void ForEach(Action<T> action) => _list.ForEach(action);

        public List<T> GetRange(int index, int count) => _list.GetRange(index, count);

        public int LastIndexOf(T item) => _list.LastIndexOf(item);

        public int LastIndexOf(T item, int index) => _list.LastIndexOf(item, index);

        public int LastIndexOf(T item, int index, int count) => _list.LastIndexOf(item, index, count);

        public int RemoveAll(Predicate<T> match) {
            var items = _list.FindAll(match);
            foreach (var item in items) {
                _ops.Add(ListOp.CreateRemove(item));
            }
            var count = items.Count;
            CommitChanges();
            return count;
        }

        public void Reverse() => _list.Reverse();

        public void Reverse(int index, int count) => _list.Reverse(index, count);

        public void Sort() => _list.Sort();

        public void Sort(Comparison<T> comparison) => _list.Sort(comparison);

        public void Sort(IComparer<T> comparer) => _list.Sort(comparer);

        public void Sort(int index, int count, IComparer<T> comparer) => _list.Sort(index, count, comparer);

        public T[] ToArray() => _list.ToArray();

        public void TrimExcess() {
            throw new NotImplementedException();
        }

        public bool TrueForAll(Predicate<T> match) => _list.TrueForAll(match);

        public T this[int index] {
            get {
                return _list[index];
            }
            set {
                _list[index] = value;
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly { get; } = false;

        public void Add(T item) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateAdd(item));
            } else {
                _list.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> collection) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateAddRange(collection));
            } else {
                _list.AddRange(collection);
            }
        }

        public void Clear() {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateClear());
            } else {
                _list.Clear();
            }
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public void CopyTo(T[] array) => _list.CopyTo(array);

        public void CopyTo(int index, T[] array, int arrayIndex, int count) => _list.CopyTo(index, array, arrayIndex, count);

        public IEnumerator<T> GetEnumerator() {
            ++_enumeratorCount;
            return new Enumerator(this);
        }

        public int IndexOf(T item) => _list.IndexOf(item);

        public int IndexOf(T item, int index) => _list.IndexOf(item, index);

        public int IndexOf(T item, int index, int count) => _list.IndexOf(item, index, count);

        public void Insert(int index, T item) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateInsert(index, item));
            } else {
                _list.Insert(index, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateInsertRange(index, collection));
            } else {
                _list.InsertRange(index, collection);
            }
        }

        public bool Remove(T item) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateRemove(item));
                return true;
            } else {
                return _list.Remove(item);
            }
        }

        public void RemoveAt(int index) {
            if (IsEnumerating()) {
                _ops.Add(ListOp.CreateRemoveAt(index));
            } else {
                _list.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void OnEnumeratorDisposed(Enumerator enumerator) {
            lock (SyncObject) {
                --_enumeratorCount;
                if (_enumeratorCount <= 0) {
                    CommitChanges();
                    _enumeratorCount = 0;
                }
            }
        }

        private void CommitChanges() {
            if (_ops.Count <= 0) {
                return;
            }
            for (var i = 0; i < _ops.Count; ++i) {
                var op = _ops[i];
                switch (op.Type) {
                    case ListOpType.Add:
                        _list.Add(op.Item);
                        break;
                    case ListOpType.AddRange:
                        _list.AddRange(op.Collection);
                        break;
                    case ListOpType.Clear:
                        _list.Clear();
                        break;
                    case ListOpType.Insert:
                        Debug.Print("This method is unsafe in iterations.");
                        _list.Insert(op.Index, op.Item);
                        break;
                    case ListOpType.InsertRange:
                        Debug.Print("This method is unsafe in iterations.");
                        _list.InsertRange(op.Index, op.Collection);
                        break;
                    case ListOpType.Remove:
                        _list.Remove(op.Item);
                        break;
                    case ListOpType.RemoveAt:
                        _list.RemoveAt(op.Index);
                        break;
                    case ListOpType.RemoveRange:
                        _list.RemoveRange(op.Index, op.Count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _ops.Clear();
        }

        private bool IsEnumerating() => _enumeratorCount > 0;

        private List<T> _list;

        private List<ListOp> _ops = new List<ListOp>();

        private readonly object SyncObject = new object();

        private int _enumeratorCount = 0;

    }
}
