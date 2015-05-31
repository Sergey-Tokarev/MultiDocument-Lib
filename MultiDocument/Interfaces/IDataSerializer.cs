using System;

namespace MultiDocument.Interfaces
{
    public interface IDataSerializer
    {
        /// <summary>
        /// Serializes the object to the byte array.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>A byte array that represents serialized object</returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// Deserializes the specified byte array into an object
        /// </summary>
        /// <param name="buffer">An array of bytes that represents an object</param>
        /// <param name="type">The type of deserialized object</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(byte[] buffer, Type type);
    }
}
