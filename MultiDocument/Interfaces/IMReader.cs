using System;
using System.Collections.Generic;


namespace MultiDocument.Interfaces
{
    public interface IMReader<T> : IMStream where T : new()
    {
        /// <summary>
        /// Reads the record at specified position
        /// </summary>
        /// <param name="pos">An index of the record to read</param>
        /// <returns>Return the record that has been read from a stream</returns>
        T Read(int pos);

        /// <summary>
        /// Reads all records from a stream
        /// </summary>
        /// <returns>A list of all records that has been read from a stream</returns>
        IList<T> ReadAll();
    }
}
