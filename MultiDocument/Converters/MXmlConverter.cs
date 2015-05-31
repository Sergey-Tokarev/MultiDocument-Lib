using MultiDocument.Writers;
using System;
using System.Collections.Generic;
using System.IO;


namespace MultiDocument.Converters
{
    public class MXmlConverter<T> : BasicDataConverter<T, MXmlWriter<T>> where T : new()
    {
        #region Constructors

        public MXmlConverter() : base()
        {
        }

        public MXmlConverter(string path) : base(path)
        {
        }

        #endregion Constructors

        #region Methods

        public Stream GetDocument()
        {
            this.writer.Flush();

            MemoryStream memoryDocumentStream = new MemoryStream();

            using (FileStream stream = new FileStream(this.path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.CopyTo(memoryDocumentStream);
            }

            return memoryDocumentStream;
        }

        #endregion Methods

        #region Help methods

        protected override void InitializeWriter()
        {
            this.writer = new MXmlWriter<T>(this.path);
        }

        #endregion Help methods
    }
}
