namespace Primary.Data
{
    public class ValueType
    {
        protected ValueType(string value) { Value = value; }
        public string Value { get; }

        public override bool Equals(object obj) { return ((ValueType)obj).Value == Value; }
        public override int GetHashCode() { return (Value != null ? Value.GetHashCode() : 0); }

        public static bool operator ==(ValueType a, ValueType b) { return a.Equals(b); }
        public static bool operator !=(ValueType a, ValueType b) { return !a.Equals(b); }

        public override string ToString() { return Value; }
    }
}
