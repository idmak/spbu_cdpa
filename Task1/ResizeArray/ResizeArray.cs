using System;

namespace ResizeArray
{
    public class ResizeArray<T>
    {
        private const int DEFAULT_SIZE = 10;
        private int capacity;
        public int Count { get; private set; }

        private Node<T> head;
        public ResizeArray()
        {
            head = new Node<T>(DEFAULT_SIZE);
            Count = 0;
            capacity = DEFAULT_SIZE;
        }

        public T this[int index] { get { return getItem(index); } set { getItem(index) = value; } }

        public void Append(T value)
        {
            Count++;
            if (Count > capacity)
            {
                Node<T> temp = head;
                while (temp.Next != null)
                    temp = temp.Next;
                temp.Next = new Node<T>(DEFAULT_SIZE);
                capacity += DEFAULT_SIZE;
            }
            this[Count - 1] = value;
        }

        private ref T getItem(int index)
        {
            if (index >= Count)
                throw new IndexOutOfRangeException();
            int i = 0;
            Node<T> temp = head;
            while (i != index / DEFAULT_SIZE)
            {
                i++;
                temp = temp.Next;
            }
            return ref temp.Value[index % DEFAULT_SIZE];
        }
    }
}
