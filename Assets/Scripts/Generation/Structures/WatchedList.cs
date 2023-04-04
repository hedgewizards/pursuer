using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    public class WatchedList<T> : List<T>
    {
        public EventHandler OnAdd;
        /// <summary>
        /// The argument is the objects index prior to removal
        /// </summary>
        public EventHandler<int> OnRemove;

        public new void Add(T item)
        {
            base.Add(item);
            OnAdd?.Invoke(this, null);
        }

        public new void Remove(T item)
        {
            int index = base.IndexOf(item);
            base.RemoveAt(index);
            OnRemove?.Invoke(this, index);
        }
    }
}
