using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MultiDocument.Writers
{
    public class MXmlWriter<T> : IMWriter<T> where T : new()
    {
        #region Members

        private XDocument doc;
        private string xmlPath;

        #endregion Members

        #region Constructors

        public MXmlWriter(string xmlPath)
        {
            this.xmlPath = xmlPath;

            if (File.Exists(this.xmlPath) && (new FileInfo(this.xmlPath).Length != 0))
            {
                // try to load and validate existing non empty document

                this.doc = XDocument.Load(xmlPath);

                if (!XSDMarkupHelper<ProcessableAttribute>.ValidateXmlDocument(this.doc, typeof(T)))
                {
                    throw new MultiDocumentException(string.Format("XML document {0} doesn't satisfy XSD schema for storing records of type {1}", xmlPath, typeof(T)));
                }
            }
            else
            {
                // don't try to load XDocument and just create a valid XML document 
                // with root element

                this.doc = new XDocument(new XElement("Document"));
            }
        }

        #endregion Constructors

        #region IMWriter<T> implementation

        public void Add(T record)
        {
            XElement recordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(record);
            doc.Root.Add(recordElement);
        }

        public void Add(IEnumerable<T> records)
        {
            foreach(T record in records)
            {
                XElement recordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(record);
                doc.Root.Add(recordElement);
            }
        }

        public void Replace(int pos, T newRecord)
        {
            if (pos < 0 || pos >= this.Count)
            {
                throw new ArgumentOutOfRangeException("pos", "Position is out of range");
            }

            string elemName = typeof(T).Name;
            IEnumerable<XElement> elems = doc.Root.Elements(elemName);

            int position = 0;
            foreach(XElement el in elems)
            {
                if(position == pos)
                {
                    XElement newRecordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(newRecord);
                    el.ReplaceWith(newRecordElement);
                    return;// DON'T DELETE RETURN
                }
                ++position;
            }
        }

        public void Replace(T oldRecord, T newRecord)
        {
            XElement oldRecordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(oldRecord);
            string elemName = typeof(T).Name;

            IEnumerable<XElement> elems = doc.Root.Elements(elemName).
                Where(e => XMLHelper<T, ProcessableAttribute>.CompareXElements(e, oldRecordElement));


            if (elems.Count() != 0)
            {
                XElement newRecordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(newRecord);

                foreach (XElement el in elems)
                {
                    el.ReplaceWith(newRecordElement);
                }
            }
        }

        public void Remove(int pos)
        {
            if (pos < 0 || pos >= this.Count)
            {
                throw new ArgumentOutOfRangeException("pos", "Position is out of range");
            }

            string elemName = typeof(T).Name;
            IEnumerable<XElement> elems = doc.Root.Descendants(elemName);

            int position = 0;
            foreach (XElement el in elems)
            {
                if (position == pos)
                {
                    el.Remove();
                    return;// DON'T DELETE RETURN
                }
                ++position;
            }
        }

        public void Remove(T record)
        {
            XElement recordElement = XMLHelper<T, ProcessableAttribute>.CreateXElement(record);
            string elemName = typeof(T).Name;

            IEnumerable<XElement> elems = doc.Root.Descendants(elemName).
                Where(e => XMLHelper<T, ProcessableAttribute>.CompareXElements(e, recordElement));

            elems.Remove();
        }

        public int Count
        {
            get 
            { 
                string elemName = typeof(T).Name;
                return doc.Root.Elements(elemName).Count();
            }
        }

        public void Clear()
        {
            string elemName = typeof(T).Name;

            IEnumerable<XElement> elems = doc.Root.Descendants(elemName);
            elems.Remove();
        }

        public void Flush()
        {
            doc.Save(this.xmlPath);
        }

        #endregion IMWriter<T> implementation
    }
}
