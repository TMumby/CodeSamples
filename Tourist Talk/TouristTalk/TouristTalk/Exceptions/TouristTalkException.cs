using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouristTalk.Exceptions
{
    public class TouristTalkException : Exception
    {
        public TouristTalkException()
            : base() { }

        public TouristTalkException(string message)
            : base(message) { }

        public TouristTalkException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public TouristTalkException(string message, Exception innerException)
            : base(message, innerException) { }

        public TouristTalkException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}