using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Exceptions
{
    public class MappingException : InvalidOperationException
    {
        public MappingException() : base() { }

        public MappingException(string message) : base(message) { }

        public MappingException(string message, Exception ex) : base(message, ex) { }
    }
}
