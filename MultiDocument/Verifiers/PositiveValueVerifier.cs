using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Verifiers
{
    public class PositiveValueVerifier : IDataVerifier
    {
        #region IDataVerifier implementation

        public void Verify(object obj)
        {
            Type type = obj.GetType();

            if(!SerializationHelper.IsPrimitiveType(type) || type == typeof(System.String) || type == typeof(DateTime))
            {
                throw new MultiDocumentException(string.Format("The type {0} can't be used in positive value check", type));
            }

            if(!OperationsHelper.IsPositive(obj))
            {
                throw new MultiDocumentException(string.Format("The value {0} is not positive", obj));
            }
        }

        #endregion IDataVerifier implementation
    }
}
