using System;

namespace MultiDocument.Interfaces
{
    public abstract class DataSerializerFactory
    {
        /// <summary>
        /// Gets the data serializer for specified type
        /// </summary>
        /// <param name="type">The type of object to get data serializer for</param>
        /// <returns>Returns the data serializer for specified type</returns>
        public abstract IDataSerializer GetDataSerializer(Type type);
    }
}
