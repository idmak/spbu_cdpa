using System;

namespace MatrixProcessor
{
    public interface IOrder
    {
        public bool LessOrEqual(IOrder Item);
    }
}
