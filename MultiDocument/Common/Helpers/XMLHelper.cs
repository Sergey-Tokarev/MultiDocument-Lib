using MultiDocument.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MultiDocument.Common.Helpers
{
    public class XMLHelper<T, AttrType> where T : new() where AttrType : ProcessableAttribute
    {
        #region Methods

        public static XElement CreateXElement(T record)
        {
            string rootElementName = typeof(T).Name;
            XElement rootElement = new XElement(rootElementName);

            ExportProperties(rootElement, record);
            ExportFields(rootElement, record);

            return rootElement;
        }

        public static bool CompareXElements(XElement firstElement, XElement secondElement)
        {
            XElement a = NormalizeElement(firstElement);
            XElement b = NormalizeElement(secondElement);

            return XElement.DeepEquals(a, b);
        }

        #endregion Methods

        #region Help methods

        private static void ExportProperties(XElement rootElement, T record)
        {
            Type type = typeof(T);

            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(propInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        XElement element;

                        if (attr is AttrType && !string.IsNullOrEmpty((attr as AttrType).Alias))
                        {
                            element = new XElement((attr as AttrType).Alias);
                        }
                        else
                        {
                            element = new XElement(propInfo.Name);
                        }

                        object value = propInfo.GetValue(record);

                        IDataVerifier verifier = (attr as AttrType).DataVerifier;
                        if (verifier != null)
                        {
                            verifier.Verify(value);
                        }

                        element.Value = value.ToString();
                        rootElement.Add(element);
                    }
                }
            }
        }

        private static void ExportFields(XElement rootElement, T record)
        {
            Type type = typeof(T);

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attr.GetType() == typeof(AttrType))
                    {
                        XElement element;

                        if (attr is AttrType && !string.IsNullOrEmpty((attr as AttrType).Alias))
                        {
                            element = new XElement((attr as AttrType).Alias);
                        }
                        else
                        {
                            element = new XElement(fieldInfo.Name);
                        }

                        object value = fieldInfo.GetValue(record);

                        IDataVerifier verifier = (attr as AttrType).DataVerifier;
                        if (verifier != null)
                        {
                            verifier.Verify(value);
                        }

                        element.Value = value.ToString();
                        rootElement.Add(element);
                    }
                }
            }
        }

        private static XElement NormalizeElement(XElement element)
        {
            if (element.HasElements)
            {
                return new XElement(element.Name, element.Attributes().
                    Where(a => a.Name.Namespace == XNamespace.Xmlns).
                    OrderBy(a => a.Name.ToString()), element.Elements().
                    OrderBy(a => a.Name.ToString()).
                    Select(e => NormalizeElement(e)));
            }

            if (element.IsEmpty || string.IsNullOrEmpty(element.Value))
            {
                return new XElement(element.Name, element.Attributes().OrderBy(a => a.Name.ToString()));
            }

            return new XElement(element.Name, element.Attributes().OrderBy(a => a.Name.ToString()), element.Value);
        }

        #endregion Help methods
    }
}
