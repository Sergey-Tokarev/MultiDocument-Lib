using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Common.Helpers
{
    public class OperationsHelper
    {
        #region Methods

        public static bool CompareByteArrays(byte[] buf1, byte[] buf2)
        {
            int length = buf1.Length;

            if (length != buf2.Length)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (buf1[i] != buf2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPositive(object value)
        {
            Type type = value.GetType();
            
            if (type == typeof(System.Byte) ||
                type == typeof(System.Char) ||
                type == typeof(System.UInt16) ||
                type == typeof(System.UInt32) ||
                type == typeof(System.UInt64))
            {
                return true;
            }
            else if (type == typeof(System.Decimal))
            {
                return (System.Decimal)value >= 0;
            }
            else if (type == typeof(System.Double))
            {
                return (System.Double)value >= 0;
            }
            else if (type == typeof(System.Int16))
            {
                return (System.Int16)value >= 0;
            }
            else if (type == typeof(System.Int32))
            {
                return (System.Int32)value >= 0;
            }
            else if (type == typeof(System.Int64))
            {
                return (System.Int64)value >= 0;
            }
            else if (type == typeof(System.SByte))
            {
                return (System.SByte)value >= 0;
            }
            else if (type == typeof(System.Single))
            {
                return (System.Single)value >= 0;
            }
            else
            {
                throw new MultiDocumentException(string.Format("The type {0} can't be used in positive value check", type));
            }
        }

        #endregion Methods
    }
}
