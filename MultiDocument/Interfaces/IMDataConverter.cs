using System;
using System.Collections.Generic;


namespace MultiDocument.Interfaces
{
    public interface IMDataConverter<T> where T : new()
    {
        /// <summary>
        /// Converts a value of type T and adds converted value to the result document
        /// </summary>
        /// <param name="value">The value to be converted</param>
        void Convert(T value);

        /// <summary>
        /// Clears all result document, so you can start another conversion process
        /// </summary>
        void ClearDocument();

        /// <summary>
        /// Flushes all converted data to file
        /// </summary>
        void Flush();
    }
}
