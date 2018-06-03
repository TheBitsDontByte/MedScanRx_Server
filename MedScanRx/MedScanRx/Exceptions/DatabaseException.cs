using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Exceptions
{
    public class DatabaseException : InvalidOperationException
    {
        public DatabaseException() : base() { }

        public DatabaseException(string message) : base(message) { }

        public DatabaseException(string message, Exception ex) : base(message, ex) { }
    }
}
