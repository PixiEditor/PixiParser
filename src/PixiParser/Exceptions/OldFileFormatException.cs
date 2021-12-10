using System;

namespace PixiEditor.Parser;

[Serializable]
public class OldFileFormatException : Exception
{
    public OldFileFormatException() { }
    public OldFileFormatException(string message) : base(message) { }
    public OldFileFormatException(string message, Exception inner) : base(message, inner) { }
    protected OldFileFormatException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
