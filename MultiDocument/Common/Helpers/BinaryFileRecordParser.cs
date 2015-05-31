using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Common.Helpers
{
    public class BinaryFileRecordParser<T, AttrType>
        where T : new()
        where AttrType : ProcessableAttribute
    {
        #region Members

        private Stream stream;
        private int recordsCount = 0;
        private byte[] signature = { 0x25, 0x26 };
        private const int recordsCountSize = sizeof(int);

        #endregion Members

        #region Constructors

        public BinaryFileRecordParser(Stream stream)
        {
            this.stream = stream;

            if (this.stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!BinaryHelper<T, AttrType>.ValidateRecordTypes())
            {
                throw new MultiDocumentException(string.Format("The record type {0} contains not supported types", typeof(T)));
            }

            ValidateHeader();
        }

        #endregion Constructors

        #region Methods

        public List<T> GetAllRecords()
        {
            List<T> records = new List<T>();

            if (this.recordsCount == 0)
            {
                return records;
            }

            Dictionary<int, object> rangedFields = new Dictionary<int, object>();
            List<object> nonRangedFields = new List<object>();

            RecordParser<ProcessableAttribute>.GetProcessingFields(typeof(T), rangedFields, nonRangedFields);

            using (BinaryReader reader = new BinaryReader(this.stream))
            {
                int count = 0;
                reader.BaseStream.Position = this.signature.Length + recordsCountSize;

                while (count != this.recordsCount)
                {
                    T record = default(T);
                    
                    if(record == null)
                    {
                        record = Activator.CreateInstance<T>();
                    }

                    foreach (var kvp in rangedFields)
                    {
                        ProcessField(record, kvp.Value, reader);
                    }

                    foreach (var field in nonRangedFields)
                    {
                        ProcessField(record, field, reader);
                    }

                    records.Add(record);
                    ++count;
                }
            }

            return records;
        }

        public T GetRecord(int pos)
        {
            if (pos < 0 || pos >= this.recordsCount)
            {
                throw new MultiDocumentException(string.Format("Invalid argument value. Position pos = {0} cannot be less than 0 or greater than records count", pos));
            }

            Dictionary<int, object> rangedFields = new Dictionary<int, object>();
            List<object> nonRangedFields = new List<object>();

            RecordParser<ProcessableAttribute>.GetProcessingFields(typeof(T), rangedFields, nonRangedFields);

            T record;

            using (BinaryReader reader = new BinaryReader(this.stream))
            {
                int count = 0;
                reader.BaseStream.Position = this.signature.Length + recordsCountSize;

                do
                {
                    record = default(T);

                    if (record == null)
                    {
                        record = Activator.CreateInstance<T>();
                    }

                    foreach (var kvp in rangedFields)
                    {
                        ProcessField(record, kvp.Value, reader);
                    }

                    foreach (var field in nonRangedFields)
                    {
                        ProcessField(record, field, reader);
                    }

                    ++count;
                } while (count - 1 != pos);
            }

            return record;
        }

        public int GetRecordsCount()
        {
            return this.recordsCount;
        }

        #endregion Methods

        #region Help methods

        private void ProcessField(T record, object field, BinaryReader reader)
        {
            Attribute[] attributes = null;
            PropertyInfo pi = null;
            FieldInfo fi = null;

            if (field is PropertyInfo)
            {
                pi = (PropertyInfo)field;
                fi = null;
                attributes = Attribute.GetCustomAttributes(pi);
            }
            else if (field is FieldInfo)
            {
                fi = (FieldInfo)field;
                pi = null;
                attributes = Attribute.GetCustomAttributes(fi);
            }

            SetProcessedField(record, pi, fi, attributes, reader);
        }

        private void SetProcessedField(T record, PropertyInfo pi, FieldInfo fi, Attribute[] attributes, BinaryReader reader)
        {
            if (pi == null && fi == null)
            {
                throw new MultiDocumentException("Both pi and fi parameters have null value.");
            }

            Type type = pi != null ? pi.PropertyType : fi.FieldType;

            foreach (Attribute attr in attributes)
            {
                if (attr.GetType() == typeof(AttrType))
                {
                    if ((attr as AttrType).SerializerFactory == null)
                    {
                        throw new MultiDocumentException("SerializerFactory property is null");
                    }

                    IDataSerializer serializer = (attr as AttrType).SerializerFactory.GetDataSerializer(type);
                    int bufferLength = 0;

                    if (type == typeof(System.String))
                    {
                        bufferLength = reader.ReadInt16() * sizeof(char);
                    }
                    else
                    {
                        bufferLength = SerializationHelper.GetMinRequiredTypeSizeInFile(type);
                    }

                    if (bufferLength == 0)
                    {
                        break;
                    }

                    byte[] buffer = ReadNextBlock(bufferLength);
                    object value = serializer.Deserialize(buffer, type);

                    IDataVerifier verifier = (attr as AttrType).DataVerifier;

                    if(verifier != null)
                    {
                        verifier.Verify(value);
                    }

                    if (pi != null)
                    {
                        pi.SetValue(record, Convert.ChangeType(value, type), null);
                    }
                    else
                    {
                        fi.SetValue(record, Convert.ChangeType(value, type));
                    }

                    break;
                }
            }
        }

        private void ValidateHeader()
        {
            int minFileLen = this.signature.Length + recordsCountSize;

            if (this.stream.Length < minFileLen)
            {
                throw new MultiDocumentException(string.Format("The minimum file size should be at least {0} byte", minFileLen));
            }

            this.stream.Position = 0;
            byte[] fileSignature = ReadNextBlock(this.signature.Length);

            if (!OperationsHelper.CompareByteArrays(this.signature, fileSignature))
            {
                throw new MultiDocumentException(string.Format("The file has invalid signature"));
            }

            this.stream.Position = 0;
            byte[] buffer = ReadNextBlock(this.stream.Length);
            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(memStream))
                {
                    reader.BaseStream.Position = this.signature.Length;
                    this.recordsCount = reader.ReadInt32();

                    if (this.recordsCount < 0)
                    {
                        throw new MultiDocumentException(string.Format("The recordCount = {0} should be positive", recordsCount));
                    }

                    // Check minimum required file size

                    long minRequiredFileSize = minFileLen + this.recordsCount * CalculateMinimumRecordSize();
                    if (this.stream.Length < minRequiredFileSize)
                    {
                        throw new MultiDocumentException(string.Format("The minimum required file size should be at least {0} byte", minRequiredFileSize));
                    }
                }
            }
        }

        public int CalculateMinimumRecordSize()
        {
            int minSize = 0;

            ICollection<PropertyInfo> props = BinaryHelper<T, AttrType>.GetProcessingProperties();
            ICollection<FieldInfo> fields = BinaryHelper<T, AttrType>.GetProcessingFields();

            foreach (PropertyInfo pi in props)
            {
                minSize += SerializationHelper.GetMinRequiredTypeSizeInFile(pi.PropertyType);
            }

            foreach (FieldInfo fi in fields)
            {
                minSize += SerializationHelper.GetMinRequiredTypeSizeInFile(fi.FieldType);
            }

            return minSize;
        }

        private byte[] ReadNextBlock(long size)
        {
            if (this.stream.Length - this.stream.Position >= size)
            {
                byte[] buffer = new byte[size];
                this.stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }

            throw new MultiDocumentException("Attempt to read data from the end of stream");
        }

        #endregion Help methods
    }
}
