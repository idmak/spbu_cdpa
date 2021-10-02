using System;

namespace MatrixProcessor
{
    public class Boolean : ISemiRing, IOrder, ISerializable
    {
        public bool Value { get; private set; }

        public Boolean()
        {
            this.Value = false;
        }

        public Boolean(bool Value)
        {
            this.Value = Value;
        }

        public ISemiRing Add(ISemiRing Addition)
        {
            return new Boolean((Addition as Boolean).Value || this.Value);
        }

        public ISemiRing Multiply(ISemiRing Factor)
        {
            return new Boolean((Factor as Boolean).Value && this.Value);
        }

        public void FromWord(string word)
        {
            if (word.Equals("t"))
                this.Value = true;
            else if (word.Equals("f"))
                this.Value = false;
            else
                throw new ArgumentException("Boolean can be restored only from 't' or 'f'");
        }

        public string ToWord()
        {
            return this.Value ? "t" : "f";
        }

        public bool LessOrEqual(IOrder Item)
        {
            return this.Value || !(Item as Boolean).Value; // Boolean algebra magic
        }

        public override bool Equals(object obj)
        {
            return Value.Equals((obj as Boolean).Value);
        }
    }
}
