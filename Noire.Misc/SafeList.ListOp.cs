using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Misc {
    public partial class SafeList<T> {

        private class ListOp {

            private ListOp() {
            }

            public static ListOp CreateAdd(T data) {
                var op = new ListOp();
                op.Type = ListOpType.Add;
                op.Item = data;
                return op;
            }

            public static ListOp CreateAddRange(IEnumerable<T> item) {
                var op = new ListOp();
                op.Type = ListOpType.AddRange;
                op.Collection = item;
                return op;
            }

            public static ListOp CreateInsert(int index, T item) {
                var op = new ListOp();
                op.Type = ListOpType.Insert;
                op.Item = item;
                op.Index = index;
                return op;
            }

            public static ListOp CreateInsertRange(int index, IEnumerable<T> collection) {
                var op = new ListOp();
                op.Type = ListOpType.InsertRange;
                op.Collection = collection;
                op.Index = index;
                return op;
            }

            public static ListOp CreateRemove(T item) {
                var op = new ListOp();
                op.Type = ListOpType.Remove;
                op.Item = item;
                return op;
            }

            public static ListOp CreateRemoveAt(int index) {
                var op = new ListOp();
                op.Type = ListOpType.RemoveAt;
                op.Index = index;
                return op;
            }

            public static ListOp CreateRemoveRange(int index, int count) {
                var op = new ListOp();
                op.Type = ListOpType.RemoveRange;
                op.Index = index;
                op.Count = count;
                return op;
            }

            public static ListOp CreateClear() {
                var op = new ListOp();
                op.Type = ListOpType.Clear;
                return op;
            }

            public ListOpType Type { get; private set; }
            public T Item { get; private set; }
            public IEnumerable<T> Collection { get; private set; }
            public int Index { get; private set; }
            public int Count { get; private set; }

        }

    }
}
