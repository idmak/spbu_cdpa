using System;

namespace MatrixProcessor
{
    public class NaturalNumber : ISemiRing, IOrder, ISerializable
    {
        public uint Value { get; private set; }

        public NaturalNumber()
        {
            this.Value = 0;
        }

        public NaturalNumber(uint Number)
        {
            this.Value = Number;
        }
        public ISemiRing Add(ISemiRing Addition)
        {
            return new NaturalNumber((Addition as NaturalNumber).Value + this.Value);
        }

        public ISemiRing Multiply(ISemiRing Factor)
        {
            return new NaturalNumber((Factor as NaturalNumber).Value * this.Value);
        }

        public void FromWord(String word)
        {
            this.Value = UInt32.Parse(word);
        }

        public string ToWord()
        {
            return this.Value.ToString();
        }

        public bool LessOrEqual(IOrder Item)
        {
            return this.Value <= (Item as NaturalNumber).Value;
        }

        public override bool Equals(object obj)
        {
            return (obj as NaturalNumber).Value.Equals(Value);
        }
    }
}
