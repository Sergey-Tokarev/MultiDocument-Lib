using MultiDocument.Converters;
using MultiDocument.Interfaces;
using MultiDocument.Readers;
using MultiDocument.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Factories
{
    public class BasicDocumentFactory<T> : IMDocumentFactory<T> where T : new()
    {
        #region IMDocumentFactory<T> implementation

        public IMWriter<T> GetWriter(string path, string format)
        {
            switch (format)
            {
                case "binary":
                    return new MBinaryWriter<T>(path);

                case "xml":
                    return new MXmlWriter<T>(path);

                default:
                    throw new MultiDocumentException(string.Format("{0} is not supported format"));
            }
        }

        public IMReader<T> GetReader(string path, string format)
        {
            switch (format)
            {
                case "binary":
                    return new MBinaryReader<T>(path);

                case "xml":
                    return new MXmlReader<T>(path);

                default:
                    throw new MultiDocumentException(string.Format("{0} is not supported format"));
            }
        }

        public IMDataConverter<T> GetConverter(string path, string format)
        {
            switch (format)
            {
                case "binary":
                    return new MBinaryConverter<T>(path);

                case "xml":
                    return new MXmlConverter<T>(path);

                default:
                    throw new MultiDocumentException(string.Format("{0} is not supported format"));
            }
        }

        public IEnumerable<string> SupportedFormats
        {
            get
            {
                return new string[]
                { 
                    "binary" , 
                    "xml" 
                };
            }
        }

        #endregion IMDocumentFactory<T> implementation
    }
}
