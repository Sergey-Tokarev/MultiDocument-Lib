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
    public class BinaryHelper<T, AttrType>
        where T : new()
        where AttrType : ProcessableAttribute
    {
        private static byte[] signature = { 0x25, 0x26 };
        private const int recordsCountSize = sizeof(int);

        #region Methods

        public static void WriteRecords(List<T> records, Stream stream)
        {
            BinaryFileRecordParser<T, AttrType> parser = new BinaryFileRecordParser<T, AttrType>(stream);

            if (records.Count == 0)
            {
                return;
            }

            Dictionary<int, object> rangedFields = new Dictionary<int, object>();
            List<object> nonRangedFields = new List<object>();

            RecordParser<ProcessableAttribute>.GetProcessingFields(typeof(T), rangedFields, nonRangedFields);

            stream.Seek(0, SeekOrigin.End);

            foreach (T record in records)
            {
                foreach (var kvp in rangedFields)
                {
                    WriteField(record, kvp.Value, stream);
                }

                foreach (var field in nonRangedFields)
                {
                    WriteField(record, field, stream);
                }
            }

            int recordsCount = parser.GetRecordsCount() + records.Count;
            stream.Position = signature.Length;
            byte[] recordsCountBuffer = BitConverter.GetBytes(recordsCount);
            stream.Write(recordsCountBuffer, 0, recordsCountBuffer.Length);
        }

        public static bool CompareElements(T firstElement, T secondElement)
        {
            ICollection<PropertyInfo> props = GetProcessingProperties();
            ICollection<FieldInfo> fields = GetProcessingFields();

            foreach (PropertyInfo pi in props)
            {
                object firstValue = pi.GetValue(firstElement);
                object secondValue = pi.GetValue(secondElement);

                if (!firstValue.Equals(secondValue))
                {
                    return false;
                }
            }

            foreach (FieldInfo fi in fields)
            {
                object firstValue = fi.GetValue(firstElement);
                object secondValue = fi.GetValue(secondElement);

                if (!firstValue.Equals(secondValue))
                {
                    return false;
                }
            }

            return true;
        }

        public static ICollection<PropertyInfo> GetProcessingProperties()
        {
            Type type = typeof(T);
            List<PropertyInfo> processedProps = new List<PropertyInfo>();

            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(propInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        processedProps.Add(propInfo);
                    }
                }
            }

            return processedProps;
        }

        public static ICollection<FieldInfo> GetProcessingFields()
        {
            Type type = typeof(T);
            List<FieldInfo> processedFields = new List<FieldInfo>();

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        processedFields.Add(fieldInfo);
                    }
                }
            }

            return processedFields;
        }

        public static bool ValidateRecordTypes()
        {
            ICollection<PropertyInfo> props = BinaryHelper<T, AttrType>.GetProcessingProperties();
            ICollection<FieldInfo> fields = BinaryHelper<T, AttrType>.GetProcessingFields();

            foreach (PropertyInfo pi in props)
            {
                if (!(pi.PropertyType == typeof(System.DateTime) || SerializationHelper.IsPrimitiveType(pi.PropertyType)))
                {
                    return false;
                }
            }

            foreach (FieldInfo fi in fields)
            {
                if (!(fi.FieldType == typeof(System.DateTime) || SerializationHelper.IsPrimitiveType(fi.FieldType)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Methods

        #region Help methods

        private static void WriteField(T record, object field, Stream writer)
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

            WriteProcessedField(record, pi, fi, attributes, writer);
        }

        private static void WriteProcessedField(T record, PropertyInfo pi, FieldInfo fi, Attribute[] attributes, Stream writer)
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

                    object value = null;

                    if (pi != null)
                    {
                        value = pi.GetValue(record);
                    }
                    else
                    {
                        value = fi.GetValue(record);
                    }

                    IDataVerifier verifier = (attr as AttrType).DataVerifier;

                    if (verifier != null)
                    {
                        verifier.Verify(value);
                    }

                    byte[] buffer = serializer.Serialize(value);
                    writer.Write(buffer, 0, buffer.Length);

                    break;
                }
            }
        }

        #endregion Help methods
    }
}
