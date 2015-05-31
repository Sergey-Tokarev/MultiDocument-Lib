using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MultiDocument.Readers
{
    public class MXmlReader<T> : IMReader<T> where T : new()
    {
        #region Members

        private IMDataConverter<T> converter;
        private XDocument doc;
        private string xmlPath;

        #endregion Members

        #region Constructors

        public MXmlReader(string xmlPath, IMDataConverter<T> converter = null)
        {
            this.xmlPath = xmlPath;
            this.converter = converter;
            this.doc = XDocument.Load(xmlPath, LoadOptions.PreserveWhitespace);

            if (!XSDMarkupHelper<ProcessableAttribute>.ValidateXmlDocument(this.doc, typeof(T)))
            {
                throw new MultiDocumentException(string.Format("XML document {0} doesn't satisfy XSD schema for storing records of type {1}", xmlPath, typeof(T)));
            }
        }

        #endregion Constructors

        #region IMReader<T> implementation

        public T Read(int pos)
        {
            if(pos < 0)
            {
                throw new MultiDocumentException(string.Format("Invalid argument pos = {0}. Position cannot be less than 0", pos));
            }

            int position = 0;
            foreach(XElement el in doc.Root.Elements())
            {
                if (el.Name == typeof(T).Name)
                {
                    if (pos == position)
                    {
                        T record = GetRecordFromElement(el);
                        return record;
                    }

                    ++position; 
                }
            }

            throw new MultiDocumentException(string.Format("Invalid record position. There is no record with pos = {0} in {1}", pos, this.xmlPath));
        }

        public IList<T> ReadAll()
        {
            List<T> records = new List<T>();

            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name == typeof(T).Name)
                {
                    T record = GetRecordFromElement(el);
                    records.Add(record);
                }
            }

            return records;
        }

        public int Count
        {
            get 
            {
                string elemName = typeof(T).Name;
                return doc.Root.Elements(elemName).Count();
            }
        }

        #endregion IMReader<T> implementation

        #region Methods

        void ConvertDocument()
        {
            if (this.converter == null)
            {
                throw new NullReferenceException("Converter didn't set for current MXmlReader.");
            }

            this.converter.ClearDocument();

            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name == typeof(T).Name)
                {
                    T record = GetRecordFromElement(el);
                    converter.Convert(record);
                }
            }

        }

        #endregion Methods

        #region Help methods

        private T GetRecordFromElement(XElement el)
        {
            if (el.Name == typeof(T).Name)
            {
                Dictionary<string, object> elems = new Dictionary<string, object>();
                foreach (XElement element in el.Elements())
                {
                    elems[element.Name.LocalName] = element.Value;
                }

                T record = default(T);

                if (record == null)
                {
                    record = Activator.CreateInstance<T>();
                }

                RecordParser<ProcessableAttribute>.ParseValues(elems, record);
                return record;
            }

            throw new MultiDocumentException(string.Format("The type of record doesn't match to the specified element"));
        }

        #endregion Help methods
    }
}
