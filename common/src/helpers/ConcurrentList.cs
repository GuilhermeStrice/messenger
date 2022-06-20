using System.Collections.Generic;

namespace Messenger.Helpers
{
    // this should work for now, can be improved
    public class ConcurrentList<T>
    {
        private List<T> list = new List<T>();

        private object lockObj = new object();

        public void Add(T t)
        {
            lock (lockObj)
            {
                list.Add(t);
            }
        }

        public void RemoveAt(int index)
        {
            lock (lockObj)
            {
                list.RemoveAt(index);
            }
        }

        public void Remove(T obj)
        {
            lock (lockObj)
            {
                list.Remove(obj);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (lockObj)
                {
                    return list[index];
                }
            }

            set
            {
                lock (lockObj)
                {
                    list[index] = value;
                }
            }
        }

        public int Length 
        { 
            get
            {
                lock (lockObj)
                {
                    return list.Count;
                }
            }
        }
    }
}