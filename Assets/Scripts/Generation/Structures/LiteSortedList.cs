using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1">Key - what this array is sorted by</typeparam>
    /// <typeparam name="T2">Value</typeparam>
    public class LiteSortedList<T1, T2> where T1 : IComparable
    {
        List<(T1 Key, T2 Value)> list;

        #region List

        public int Count => list.Count;

        #endregion

        public LiteSortedList()
        {
            list = new List<(T1 Key, T2 Value)>();
        }

        public (T1 Key, T2 Value) Last => list[list.Count - 1];
        public (T1 Key, T2 Value) First => list[0];

        public List<(T1 Key, T2 Value)> ToList()
        {
            return list;
        }

        public void OrderedInsert(T1 newKey, T2 newValue)
        {
            // find the right place to insert using binary search
            int left = 0;
            int right = list.Count;

            while (left < right)
            {
                int mid = (left + right) / 2;
                int comparison = newKey.CompareTo(list[mid].Key);
                if (comparison == 0)
                {
                    left = mid;
                    break;
                }
                else if (comparison > 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            list.Insert(left, (newKey, newValue));
        }

        public void DropLast()
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}
