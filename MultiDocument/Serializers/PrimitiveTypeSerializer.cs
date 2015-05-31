using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using System;


namespace MultiDocument.Serializers
{
    public class PrimitiveTypeSerializer : IDataSerializer
    {
        #region IDataSerializer implementation

        public byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();

            if (!SerializationHelper.IsPrimitiveType(type))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be serialized", type));
            }

            return SerializationHelper.BinarySerializePrimitiveTypeToByte(obj);
        }

        public object Deserialize(byte[] buffer, Type type)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (!SerializationHelper.IsPrimitiveType(type))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be deserialized", type));
            }

            return SerializationHelper.BinaryDeserializePrimitiveType(buffer, type);
        }

        #endregion IDataSerializer implementation
    }
}
