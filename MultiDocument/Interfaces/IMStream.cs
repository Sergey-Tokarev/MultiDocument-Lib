using System;

namespace MultiDocument.Interfaces
{
    public interface IMStream
    {
        /// <summary>
        /// Returns the number of elements in a stream
        /// </summary>
        int Count { get; }
    }
}
