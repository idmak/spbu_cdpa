namespace ResizeArray
{
    class Node<T>
    {
        public int Size { get; private set; }
        public Node<T> Next;
        public T[] Value;
        public Node(int Size)
        {
            Value = new T[Size];
            this.Size = Size;
            Next = null;
        }
    }
}