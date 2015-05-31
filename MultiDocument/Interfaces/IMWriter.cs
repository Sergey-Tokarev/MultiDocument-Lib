using System;
using System.Collections.Generic;

namespace MultiDocument.Interfaces
{
    public interface IMWriter<T> : IMStream where T : new()
    {
        /// <summary>
        /// Adds the record to the end of the stream
        /// </summary>
        /// <param name="record">The record to be added to the end of the stream </param>
        void Add(T record);

        /// <summary>
        /// Adds the records of the specified collection to the end of the stream
        /// </summary>
        /// <param name="records">The collection whose records should be added to the end of the stream</param>
        void Add(IEnumerable<T> records);

        /// <summary>
        /// Replaces the record at specified position with new record value 
        /// </summary>
        /// <param name="pos">Specifies the record position to be replaced by the new value</param>
        /// <param name="newRecord">Specifies the new record value</param>
        void Replace(int pos, T newRecord);

        /// <summary>
        /// Replaces all occurrences of a specified record in the stream with another specified record
        /// </summary>
        /// <param name="oldRecord">Specifies the old record value to be replaced by the new record value</param>
        /// <param name="newRecord">Specifies the new record value</param>
        void Replace(T oldRecord, T newRecord);

        /// <summary>
        /// Removes a record from the stream
        /// </summary>
        /// <param name="pos">The zero-based position of record to be deleted</param>
        void Remove(int pos);

        /// <summary>
        /// Removes the specified record from the stream
        /// </summary>
        /// <param name="record">The record to be removed from the stream</param>
        void Remove(T record);

        /// <summary>
        /// Removes all records from the stream
        /// </summary>
        void Clear();

        /// <summary>
        /// Flushes all document to file
        /// </summary>
        void Flush();
    }
}
