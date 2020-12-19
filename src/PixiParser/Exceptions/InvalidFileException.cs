using System;

namespace PixiEditor.Parser.Exceptions
{

    [Serializable]
    public class InvalidFileException : Exception
    {
        public InvalidFileException() { }
        public InvalidFileException(string message) : base(message) { }
        public InvalidFileException(string message, Exception inner) : base(message, inner) { }
        protected InvalidFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
