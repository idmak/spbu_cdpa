using System;

namespace MatrixProcessor
{
    public interface ISerializable
    {
        public void FromWord(string word);
        public string ToWord();
    }
}
