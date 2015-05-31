using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Interfaces
{
    public interface IDataVerifier
    {
        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="obj">The value of the object to validate.</param>
        void Verify(object obj);
    }
}
