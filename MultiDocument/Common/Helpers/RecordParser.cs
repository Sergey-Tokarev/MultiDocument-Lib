using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace MultiDocument.Common.Helpers
{
    public class RecordParser<AttrType> where AttrType : ProcessableAttribute
    {
        #region Methods

        public static bool ParseValues(IDictionary<string, object> elems, object record)
        {
            if (record == null)
            {
                throw new ArgumentException("Record parameter does not refer to any object", "record");
            }

            Dictionary<string, bool> requiredParams = new Dictionary<string, bool>();

            object value = null;

            foreach (string elemName in elems.Keys)
            {
                PropertyInfo propInfo = GetAssosiatedProperty(elemName, record);

                if (propInfo != null)
                {
                    value = elems[elemName];

                    IDataVerifier verifier = propInfo.GetCustomAttribute<AttrType>().DataVerifier;
                    if (verifier != null)
                    {
                        verifier.Verify(value);
                    }

                    propInfo.SetValue(record, Convert.ChangeType(value, propInfo.PropertyType), null);
                    continue;
                }

                FieldInfo fieldInfo = GetAssosiatedField(elemName, record);

                if (fieldInfo != null)
                {
                    value = elems[elemName];

                    IDataVerifier verifier = fieldInfo.GetCustomAttribute<AttrType>().DataVerifier;
                    if (verifier != null)
                    {
                        verifier.Verify(value);
                    }

                    fieldInfo.SetValue(record, Convert.ChangeType(value, fieldInfo.FieldType));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static void GetProcessingFields(Type type, Dictionary<int, object> rangedFields, List<object> nonRangedFields)
        {
            if (rangedFields == null)
            {
                throw new ArgumentNullException("rangedFields");
            }

            if (nonRangedFields == null)
            {
                throw new ArgumentNullException("nonRangedFields");
            }

            rangedFields.Clear();
            nonRangedFields.Clear();

            try
            {

                foreach (PropertyInfo propInfo in type.GetProperties())
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(propInfo))
                    {
                        if (attr.GetType() == typeof(AttrType))
                        {
                            if ((attr as AttrType).Rank != -1)
                            {
                                rangedFields.Add((attr as AttrType).Rank, propInfo);
                            }
                            else
                            {
                                nonRangedFields.Add(propInfo);
                            }
                        }
                    }
                }

                foreach (FieldInfo fieldInfo in type.GetFields())
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                    {
                        if (attr.GetType() == typeof(AttrType))
                        {
                            if ((attr as AttrType).Rank != -1)
                            {
                                rangedFields.Add((attr as AttrType).Rank, fieldInfo);
                            }
                            else
                            {
                                nonRangedFields.Add(fieldInfo);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The Rank property of ProcessableAttribute cannot has duplicated values");
            }
        }

        #endregion Methods

        #region Help methods

        private static PropertyInfo GetAssosiatedProperty(string elemName, object record)
        {
            Type type = record.GetType();

            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(propInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        if (attr is AttrType && ((attr as AttrType).Alias == elemName))
                        {
                            return propInfo;
                        }
                    }
                }

                if (propInfo.Name == elemName)
                {
                    return propInfo;
                }
            }

            return null;
        }

        private static FieldInfo GetAssosiatedField(string elemName, object record)
        {
            Type type = record.GetType();

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        if (attr is AttrType && ((attr as AttrType).Alias == elemName))
                        {
                            return fieldInfo;
                        }
                    }
                }

                if (fieldInfo.Name == elemName)
                {
                    return fieldInfo;
                }
            }

            return null;
        }

        #endregion Help methods
    }
}
