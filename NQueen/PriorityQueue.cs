using System;
using System.Collections.Generic;
using System.Text;

namespace NQueen
{
    public class PriorityQueue<P, V>
    {
        private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
        private int count = 0;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        
        public void Enequeue(P priority, V value)
        {
            Queue<V> q;
            if (!list.TryGetValue(priority,out q))
            {
                q = new Queue<V>();
                list.Add(priority, q);
            }
            q.Enqueue(value);
            count++;
        }

        public V Dequeue()
        {
            KeyValuePair<P, Queue<V>> kvp = new KeyValuePair<P, Queue<V>>();
            SortedDictionary<P, Queue<V>>.Enumerator e = list.GetEnumerator();
            if (e.MoveNext())
                kvp = e.Current;
            V v = kvp.Value.Dequeue();
            if (kvp.Value.Count == 0)//If Queue is empty
                list.Remove(kvp.Key);
            count--;
            return v;            
        }
    }
}
