using System;

namespace JunctionController
{ 
    /// <summary>
    /// Application Exception
    /// </summary>
public class JunctionException : Exception
    {
        public JunctionException()
            : base() { }

        public JunctionException(string message)
            : base(message) { }

        public JunctionException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public JunctionException(string message, Exception innerException)
            : base(message, innerException) { }

        public JunctionException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
