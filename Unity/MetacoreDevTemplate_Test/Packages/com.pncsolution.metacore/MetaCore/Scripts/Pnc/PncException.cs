using System;

[Serializable]
public class PncException : Exception
{
    public PncException() : base() { }
    public PncException(string message) : base(message) { }
    public PncException(string message, Exception inner) : base(message, inner) { }

    protected PncException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}