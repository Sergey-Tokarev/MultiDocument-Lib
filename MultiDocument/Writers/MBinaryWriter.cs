using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using MultiDocument.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MultiDocument.Writers
{
    public class MBinaryWriter<T> : IMWriter<T> where T : new()
    {
        #region Members

        private string filePath;
        private List<T> records = new List<T>();
        private byte[] signature = { 0x25, 0x26 };
        private const int recordsCountSize = sizeof(int);

        #endregion Members

        #region Constructors

        public MBinaryWriter(string path)
        {
            this.filePath = path;

            if (!BinaryHelper<T, ProcessableAttribute>.ValidateRecordTypes())
            {
                throw new MultiDocumentException(string.Format("The record type {0} contains not supported types", typeof(T)));
            }

            if (File.Exists(this.filePath) && (new FileInfo(this.filePath).Length != 0))
            {
                // try to load and validate existing non empty document

                MBinaryReader<T> reader = new MBinaryReader<T>(this.filePath);
                records.AddRange(reader.ReadAll());
            }
        }

        #endregion Constructors

        #region IMWriter implementation

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

        public int Count
        {
            get
            {
                return this.records.Count();
            }
        }

        public void Clear()
        {
            this.records.Clear();
        }

        public void Flush()
        {
            CreateEmptyDocument(this.filePath);
            SaveRecords(this.records);
        }

        #endregion IMWriter implementation

        #region Help methods

        private void CreateEmptyDocument(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.SetLength(0);

                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    int recordsCount = 0;
                    writer.Write(signature);
                    writer.Write(recordsCount);
                    writer.Flush();
                }
            }
        }

        private void SaveRecords(List<T> records)
        {
            using (FileStream stream = new FileStream(this.filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                BinaryHelper<T, ProcessableAttribute>.WriteRecords(records, stream);
                stream.Flush();
            }
        }

        #endregion Help methods
    }
}
