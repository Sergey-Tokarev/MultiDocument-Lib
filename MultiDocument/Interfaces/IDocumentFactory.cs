using System;
using System.Collections.Generic;


namespace MultiDocument.Interfaces
{
    public interface IMDocumentFactory<T> where T : new()
    {
        /// <summary>
        /// Gets a new instance of writer class that implements IMWriter<T> interface with specified path and file format
        /// </summary>
        /// <param name="path">  An absolute path for the file that the current IMWriter<T> object will encapsulate.</param>
        /// <param name="format">A string that determines format of file specified by path</param>
        /// <returns>A new instance of writer class that implements IMWriter<T> interface with specified path and file format</returns>
        IMWriter<T> GetWriter(string path, string format);

        /// <summary>
        /// Gets a new instance of reader class that implements IMReader<T> interface with specified path and file format
        /// </summary>
        /// <param name="path"> An absolute path for the file that the current IMReader<T> object will encapsulate.</param>
        /// <param name="format">A string that determines format of file specified by path</param>
        /// <returns>A new instance of reader class that implements IMReader<T> interface with specified  path and file format</returns>
        IMReader<T> GetReader(string path, string format);

        /// <summary>
        /// Gets a new instance of converter class that implements IMDataConverter<T> interface with specified path and file format
        /// </summary>
        /// <param name="path">An absolute path for the file that the current IMDataConverter<T> object will encapsulate.</param>          
        /// <param name="format">A string that determines format of file specified by path</param>
        /// <returns>A new instance of converter class that implements IMDataConverter<T> interface with specified path and file format</returns>
        IMDataConverter<T> GetConverter(string path, string format);

        /// <summary>
        /// Gets a collection of supported file formats
        /// </summary>
        IEnumerable<string> SupportedFormats { get; }
    }
}
