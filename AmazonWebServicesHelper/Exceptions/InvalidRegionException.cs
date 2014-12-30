using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonWebServicesHelper.Exceptions
{
    [Serializable]
    public class InvalidRegionException : Exception
    {
        public InvalidRegionException() { }
        public InvalidRegionException(string message) : base(message) { }
        public InvalidRegionException(string message, Exception inner) : base(message, inner) { }
        protected InvalidRegionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
