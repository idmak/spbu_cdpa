using System;

namespace MatrixProcessor
{
    public interface ISemiRing
        
    {
        public ISemiRing Multiply(ISemiRing Factor);
        public ISemiRing Add(ISemiRing Addition);

    }
}
