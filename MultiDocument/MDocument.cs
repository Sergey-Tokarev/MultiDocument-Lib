using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Factories;
using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument
{
    public class MDocument<T> : IMReader<T>, IMWriter<T> where T : new()
    {
        #region Members

        private string path;
        private string format;
        private IMDocumentFactory<T> documentFactory;
        private List<T> records = new List<T>();

        #endregion Members

        #region Conctructors

        public MDocument(string path, string format) : this(path, format, new BasicDocumentFactory<T>())
        {
        }

        public MDocument(string path, string format, IMDocumentFactory<T> documentFactory)
        {
            this.path = path;
            this.format = format;
            this.documentFactory = documentFactory;

            if (this.documentFactory == null)
            {
                throw new ArgumentNullException("documentFactory");
            }

            if (File.Exists(this.path) && (new FileInfo(this.path).Length != 0))
            {
                // try to load and validate existing non empty document
                IMReader<T> reader = this.documentFactory.GetReader(this.path, this.format);

                if (reader == null)
                {
                    throw new MultiDocumentException(string.Format("{0} is not supported format"));
                }

                records.AddRange(reader.ReadAll());
            }
        }

        #endregion Constructors

        #region IMReader<T> implementation

        public T Read(int pos)
        {
            if (pos < 0 || pos >= this.records.Count)
            {
                throw new MultiDocumentException(string.Format("Invalid argument pos = {0}. Position cannot be less than 0 or greater than records count", pos));
            }

            return this.records[pos];
        }

        public IList<T> ReadAll()
        {
            return this.records;
        }

        #endregion IMReader<T> implementation

        #region IMWriter<T> implementation

        public int Count
        {
            get { return records.Count; }
        }

        public void Flush()
        {
            IMWriter<T> writer = this.documentFactory.GetWriter(this.path, this.format);

            if (writer == null)
            {
                throw new MultiDocumentException(string.Format("{0} is not supported format", this.format));
            }

            writer.Clear();
            writer.Add(this.records);
            writer.Flush();
        }

        public void Add(T record)
        {
            this.records.Add(record);
        }

        public void Add(IEnumerable<T> records)
        {
            this.records.AddRange(records);
        }

        public void Replace(int pos, T newRecord)
        {
            if (pos < 0 || pos >= this.Count)
            {
                throw new ArgumentOutOfRangeException("pos", "Position is out of range");
            }

            this.records.RemoveAt(pos);
            this.records.Insert(pos, newRecord);
        }

        public void Replace(T oldRecord, T newRecord)
        {
            List<T> recordsToReplace = this.records.Where(v => BinaryHelper<T, ProcessableAttribute>.CompareElements(v, oldRecord)).ToList();

            foreach (T record in recordsToReplace)
            {
                Replace(this.records.IndexOf(record), newRecord);
            }
        }

        public void Remove(int pos)
        {
            if (pos < 0 || pos >= this.Count)
            {
                throw new ArgumentOutOfRangeException("pos", "Position is out of range");
            }

            this.records.RemoveAt(pos);
        }

        public void Remove(T record)
        {
            this.records.RemoveAll(v => BinaryHelper<T, ProcessableAttribute>.CompareElements(v, record));
        }

        public void Clear()
        {
            this.records.Clear();
        }

        #endregion IMWriter<T> implementation

        #region Methods

        public void ConvertTo(string path, string format)
        {
            if(path == this.path && format != this.format)
            {
                throw new MultiDocumentException(string.Format("You cannot convert document from {0} to {1} format specifying the same path {2}", this.format, format, path));
            }

            IMDataConverter<T> converter = this.documentFactory.GetConverter(path, format);

            if (converter == null)
            {
                throw new MultiDocumentException(string.Format("Can't convert to data. {0} is not supported format", format));
            }

            converter.ClearDocument();

            foreach(T record in this.records)
            {
                converter.Convert(record);
            }

            converter.Flush();
        }

        #endregion Methods

        #region Properties

        public IEnumerable<string> SupportedFormats
        {
            get
            {
                IEnumerable<string> supportedFormats = this.documentFactory.SupportedFormats;
                List<string> formats = new List<string>();

                if(supportedFormats != null)
                {
                    formats.AddRange(supportedFormats);
                }

                return formats;
            }
        }

        #endregion Properties
    }
}
