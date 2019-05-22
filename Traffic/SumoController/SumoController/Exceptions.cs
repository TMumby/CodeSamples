using System;


namespace SumoController
{
    /// <summary>
    /// Application Exception
    /// </summary>
    public class SumoControllerException : Exception
    {
        public SumoControllerException()
            : base() { }

        public SumoControllerException(string message)
            : base(message) { }

        public SumoControllerException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public SumoControllerException(string message, Exception innerException)
            : base(message, innerException) { }

        public SumoControllerException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
