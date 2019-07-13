using System;

namespace Primary.Serialization
{
    public class InvalidEnumStringException : Exception
    {
        private const string InvalidEnumError = "Invalid enum/string value";

        public InvalidEnumStringException() : base(InvalidEnumError) {}
    }
}