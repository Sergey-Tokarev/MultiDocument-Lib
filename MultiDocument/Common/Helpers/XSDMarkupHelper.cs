using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MultiDocument.Common.Helpers
{
    public class XSDMarkupHelper<AttrType> where AttrType : BaseAliasAttribute
    {
        #region Methods

        public static string CreateValidationMarkup(Type recordType)
        {
            if (recordType == null)
            {
                throw new ArgumentException("recordType parameter does not refer to any object", "recordType");
            }

            string rootName = "Document";
            string recordName = recordType.Name;
            string propertiesMarkup = ProcessAllProperties(recordType);
            string fieldsMarkup = ProcessAllFields(recordType);
            propertiesMarkup = string.IsNullOrEmpty(propertiesMarkup) ? "" : propertiesMarkup;
            fieldsMarkup = string.IsNullOrEmpty(fieldsMarkup) ? "" : fieldsMarkup;

            string xsdMarkup  = string.Format(@"<?xml version='1.0' encoding='utf-8'?>
                                                <xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                                                 <xsd:element name='{0}'>
                                                  <xsd:complexType>
                                                   <xsd:sequence>
                                                    <xsd:element name='{1}' maxOccurs='unbounded'>
                                                     <xsd:complexType>
                                                      <xsd:sequence>
                                                       {2}
                                                       {3}
                                                     </xsd:sequence>
                                                    </xsd:complexType>
                                                   </xsd:element>
                                                  </xsd:sequence>
                                                 </xsd:complexType>
                                                </xsd:element>
                                               </xsd:schema>", rootName, recordName, propertiesMarkup, fieldsMarkup);

            return xsdMarkup;
        }

        public static bool ValidateXmlDocument(XDocument doc, Type recordType)
        {
            if (doc == null)
            {
                throw new ArgumentException("doc parameter does not refer to any object", "doc");
            }

            string xsdMarkup = CreateValidationMarkup(recordType);

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));

            bool error = false;
            doc.Validate(schemas, (o, e) => { error = true; });

            return !error;
        }

        #endregion Methods

        #region Help methods

        private static string ProcessAllProperties(Type recordType)
        {
            Type type = recordType;
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(propInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        if (attr is AttrType && !string.IsNullOrEmpty((attr as AttrType).Alias))
                        {
                            sb.AppendLine(string.Format("<xsd:element minOccurs='1' maxOccurs='1' name='{0}' />", (attr as AttrType).Alias));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("<xsd:element minOccurs='1' maxOccurs='1' name='{0}' />", propInfo.Name));
                        }
                    }
                }
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        private static string ProcessAllFields(Type recordType)
        {
            Type type = recordType;
            StringBuilder sb = new StringBuilder();

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        if (attr is AttrType && !string.IsNullOrEmpty((attr as AttrType).Alias))
                        {
                            sb.AppendLine(string.Format("<xsd:element minOccurs='1' maxOccurs='1' name='{0}' />", (attr as AttrType).Alias));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("<xsd:element minOccurs='1' maxOccurs='1' name='{0}' />", fieldInfo.Name));
                        }
                    }
                }
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        #endregion Help methods
    }
}
