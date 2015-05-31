using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument
{
    [Serializable]
    public class MultiDocumentException : ApplicationException
    {
        public MultiDocumentException()
        {
        }

        public MultiDocumentException(string message) : base(message)
        {
        }

        public MultiDocumentException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MultiDocumentException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
