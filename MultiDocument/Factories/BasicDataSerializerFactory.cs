using MultiDocument.Interfaces;
using MultiDocument.Serializers;
using System;

namespace MultiDocument.Factories
{
    public class BasicDataSerializerFactory : PrimitiveTypeDataSerializerFactory
    {
        #region Methods

        public override IDataSerializer GetDataSerializer(Type type)
        {
            if (type == typeof(System.String))
            {
                return new UnicodeStringSerializer();
            }
            else if (type == typeof(System.DateTime))
            {
                return new DateSerializer();
            }

            return base.GetDataSerializer(type);
        }

        #endregion Methods
    }
}
