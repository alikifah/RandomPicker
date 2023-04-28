// ====================================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   28.4.2023
//    Description: A class that allows picking random objects with or without repeating
// =====================================================================================

using System;
using System.Collections.Generic;

 public class RandomPicker<T>
    {
        #region private members
        private Random rnd;
        private List<T> list = new List<T>();
        private List<T> list_done = new List<T>();
        private Queue<T> queue_done = new Queue<T>();
        private bool allowRepeating;
        private int count = 0;
        private Action onFinishPicking;
        #endregion

        #region public properties
        public int Count => count;
        public bool IsFinished => list.Count == 0;
        #endregion

        #region constructor
        public RandomPicker(T[] items, bool allowRepeating = true, Action onFinishPicking = null)
        {
            this.onFinishPicking = onFinishPicking;
            count = items.Length;
            this.allowRepeating = allowRepeating;
            rnd = new Random(DateTime.Now.Millisecond);
            if (!allowRepeating)
            {
                // remove duplicates
                foreach (var item in items)
                {
                    if (!list.Contains(item))
                        list.Add(item);
                }
            }
            else
                list.AddRange(items);
        }
        #endregion

        #region private methods
        T generateNonRepeating()
        {
            if (list.Count == 0 && list_done.Count > 0)
            {
                list.AddRange(list_done);
                list_done.Clear();
            }
            int range = list.Count;
            int index = rnd.Next(range);
            T o = list[index];
            list_done.Add(o);
            list.RemoveAt(index);
            if (list.Count == 0)
                onFinishPicking?.Invoke();

            return o;
        }
        T generateRepeating()
        {
            if (queue_done.Count > 1)
            {
                list.Add(queue_done.Dequeue());
            }
            int range = list.Count;
            int index = rnd.Next(range);
            T o = list[index];

            queue_done.Enqueue(o);
            list.RemoveAt(index);
            if (list.Count == 0)
                onFinishPicking?.Invoke();
            return o;
        }
        #endregion

        #region public methods
        public void Add(T item)
        {
            lock (list)
            {
                if (!allowRepeating)
                {
                    if (!list.Contains(item))
                        list.Add(item);
                }
                else
                    list.Add(item);
            }
        }
        public void AddRange(T[] items)
        {
            lock (list)
            {
                if (!allowRepeating)
                {
                    // remove duplicates
                    foreach (var item in items)
                    {
                        if (!list.Contains(item))
                            list.Add(item);
                    }
                }
                else
                    list.AddRange(items);
            }
        }
        public T Pick()
        {
            lock (list)
            {
                if (count == 0)
                    return default(T);
                else if (count == 1)
                    return list[0];
                else if (count == 2)
                    return generateRepeating();
                else
                {
                    if (allowRepeating)
                        return generateRepeating();
                    return generateNonRepeating();
                }
            }
        }
        public T[] Pick(int itemCount)
        {
            if (itemCount == 0)
                return Array.Empty<T>();
            List<T> temp = new List<T>();
            if (itemCount > count)
            {
                if (allowRepeating)
                    return list.ToArray();
                else
                {
                    foreach (var item in list)
                    {
                        if (!temp.Contains(item))
                            temp.Add(item);
                    }
                    return temp.ToArray();
                }
            }
            if (itemCount == 2)
            {

                for (int i = 0; i < 2; i++)
                {
                    temp.Add(generateRepeating());
                }
                return temp.ToArray();
            }
            for (int i = 0; i < itemCount; i++)
            {
                if (allowRepeating)
                    temp.Add(generateRepeating());
                else
                    temp.Add(generateNonRepeating());
            }
            return temp.ToArray();
        }
        #endregion

    }
