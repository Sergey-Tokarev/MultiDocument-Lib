using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace MultiDocument.Serializers
{
    public class UnicodeStringSerializer : IDataSerializer
    {
        #region IDataSerializer implementation

        public byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();

            if (type != typeof(System.String))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be serialized", type));
            }

            string str = obj as string;
            ushort length = (ushort)str.Length;
            byte[] lengthBuffer = BitConverter.GetBytes(length);
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(str);
            byte[] result = new byte[buffer.Length + sizeof(char)];
            lengthBuffer.CopyTo(result, 0);
            buffer.CopyTo(result, sizeof(ushort));

            return result;
        }

        public object Deserialize(byte[] buffer, Type type)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (type != typeof(System.String))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be deserialized", type));
            }

            return Encoding.Unicode.GetString(buffer);
        }

        #endregion IDataSerializer implementation
    }
}
