using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;


namespace MultiDocument.Readers
{
    public class MBinaryReader<T> : IMReader<T> where T : new()
    {
        #region Members

        private IMDataConverter<T> converter;
        private string filePath;

        #endregion Members

        #region Constructors

        public MBinaryReader(string path, IMDataConverter<T> converter = null)
        {
            this.filePath = path;
            this.converter = converter;
            
            if(!File.Exists(this.filePath))
            {
                throw new MultiDocumentException(string.Format("The file specified by path = {0} doesn't exist", this.filePath));
            }
        }

        #endregion Constructors

        #region IMReader implementation

        public T Read(int pos)
        {
            using (FileStream stream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BinaryFileRecordParser<T, ProcessableAttribute> parser = new BinaryFileRecordParser<T, ProcessableAttribute>(stream);
                return parser.GetRecord(pos);
            }
        }

        public IList<T> ReadAll()
        {
            using (FileStream stream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BinaryFileRecordParser<T, ProcessableAttribute> parser = new BinaryFileRecordParser<T, ProcessableAttribute>(stream);
                return parser.GetAllRecords();
            }
        }

        public int Count
        {
            get
            {
                using (FileStream stream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    BinaryFileRecordParser<T, ProcessableAttribute> parser = new BinaryFileRecordParser<T, ProcessableAttribute>(stream);
                    return parser.GetRecordsCount();
                }
            }
        }

        #endregion IMReader implementation

        #region Methods

        void ConvertDocument()
        {
            if (this.converter == null)
            {
                throw new NullReferenceException("Converter didn't set for current MBinaryReader.");
            }

            this.converter.ClearDocument();

            using (FileStream stream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BinaryFileRecordParser<T, ProcessableAttribute> parser = new BinaryFileRecordParser<T, ProcessableAttribute>(stream);
                List<T> records = parser.GetAllRecords();

                foreach (T record in records)
                {
                    converter.Convert(record);
                }
            }
        }

        #endregion Methods
    }
}
