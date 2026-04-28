using System;

namespace Primary
{
    public class PrimaryApiException : Exception
    {
        public string Status { get; }
        public string Description { get; }

        public PrimaryApiException(string message, string description)
            : base($"{message} ({description})")
        {
            Status = "ERROR";
            Description = description;
        }

        public PrimaryApiException(string message)
            : base(message)
        {
            Status = "ERROR";
        }
    }
}
