using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    public struct TriPair : IEquatable<TriPair>
    {
        public int n, m;

        public override bool Equals(object obj)
        {
            return obj is TriPair pair &&
                   n == pair.n &&
                   m == pair.m;
        }

        public bool Equals(TriPair other)
        {
            return n == other.n && m == other.m;
        }

        public override int GetHashCode()
        {
            int hashCode = -2087071663;
            hashCode = hashCode * -1521134295 + n.GetHashCode();
            hashCode = hashCode * -1521134295 + m.GetHashCode();
            return hashCode;
        }

        public void Justify()
        {
            int temp = Math.Min(n, m);
            m = Math.Max(n, m);
            n = temp;
        }

        public override string ToString()
        {
            return $"[{n} {m}]";
        }

        public TriPair(int n, int m)
        {
            this.n = n;
            this.m = m;
        }
    }

    public class TriangleArray<T> : IEnumerable<KeyValuePair<TriPair, T>>
    {
        public class TriangleRayEnumerator : IEnumerator<KeyValuePair<TriPair, T>>
        {
            TriangleArray<T> self;

            int currentN, currentM;

            public TriangleRayEnumerator(TriangleArray<T> _self)
            {
                self = _self;
            }

            public KeyValuePair<TriPair, T> Current
            {
                get
                {
                    return new KeyValuePair<TriPair, T>(
                        new TriPair(currentN, currentM),
                        self.AtUnsafe(currentN, currentM));
                }
            }
                

            object IEnumerator.Current
            {
                get
                {
                    return new KeyValuePair<TriPair, T>(
                        new TriPair(currentN, currentM),
                        self.AtUnsafe(currentN, currentM));
                }
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                currentN++;

                if (currentN >= currentM)
                {
                    if (currentM >= self.Size - 1)
                    {
                        return false;
                    }
                    else
                    {
                        currentM++;
                        currentN = 0;
                    }
                }
                return true;
            }

            public void Reset()
            {
                currentN = 0;
                currentM = 1;
            }
        }

        T[] array;
        int size;
        public int Size => size;

        public TriangleArray(int count)
        {
            this.size = count;
            array = new T[count * (count - 1) / 2];
        }

        public void Grow()
        {
            size = size * 2;
            Array.Resize<T>(ref array, size * (size - 1) / 2);

        }

        public List<(TriPair pair, T value)> AllPairsFor(int n)
        {
            List < (TriPair pair, T value) > results = new List<(TriPair pair, T value)>();

            for (int prev = 0; prev < n; prev++)
            {
                results.Add( (new TriPair(prev, n),AtUnsafe(prev, n)) );
            }
            for (int next = n + 1; next < Size; next++)
            {
                results.Add( (new TriPair(n, next), AtUnsafe(n, next)) );
            }

            return results;
        }

        public T this[int n, int m]
        {
            get
            {
                return AtUnsafe(Math.Min(n,m), Math.Max(n,m));
            }
            set
            {
                SetAtUnsafe(Math.Min(n, m), Math.Max(n, m), value);
            }
        }

        public T this[TriPair t]
        {
            get
            {
                t.Justify();
                return AtUnsafe(t.n, t.m);
            }
            set
            {
                t.Justify();
                SetAtUnsafe(t.n, t.m, value);
            }
        }

        /// <summary>
        /// returns the element for n & m when n <= m
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public T AtUnsafe(int n, int m)
        {
            return array[index(n, m)];
        }

        public void SetAtUnsafe(int n, int m, T val)
        {
            array[index(n, m)] = val;
        }
        
        int index(int n, int m)
        {
            return (m * (m - 1)) / 2 + n;
        }

        public IEnumerator<KeyValuePair<TriPair, T>> GetEnumerator()
        {
            return new TriangleRayEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
