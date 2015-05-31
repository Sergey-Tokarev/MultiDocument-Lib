using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using MultiDocument.Serializers;
using System;


namespace MultiDocument.Factories
{
    public class PrimitiveTypeDataSerializerFactory : DataSerializerFactory
    {
        #region Overrides

        public override IDataSerializer GetDataSerializer(Type type)
        {
            if (!SerializationHelper.IsPrimitiveType(type))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be serialized", type));
            }

            return new PrimitiveTypeSerializer();
        }

        #endregion Overrides
    }
}
