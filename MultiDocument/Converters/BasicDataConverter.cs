using MultiDocument.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Converters
{
    public abstract class BasicDataConverter<T, WriterType> : IMDataConverter<T> where T : new() where WriterType : IMWriter<T>
    {
        #region Members

        protected string path;
        protected bool deleteTmpFile = false;
        protected WriterType writer = default(WriterType);

        #endregion Members

        #region Constructors

        /// <summary>
        /// Note that constructor it's a protected constructor and class is abstract, so you can't create an instance of this class directly.
        /// This class should be used only for help purposes as base class. Also note that you should
        /// initialize writer field in derived class constructor with valid writer class that implements IMWriter<T> interface
        /// by implementig InitializeWriter abstract method
        /// </summary>
        protected BasicDataConverter() : this(Path.GetTempFileName())
        {
            this.deleteTmpFile = true;
        }

        /// <summary>
        /// Note that constructor it's a protected constructor and class is abstract, so you can't create an instance of this class directly.
        /// This class should be used only for help purposes as base class. Also note that you should
        /// initialize writer field in derived class constructor with valid writer class that implements IMWriter<T> interface
        /// by implementig InitializeWriter abstract method
        /// </summary>
        protected BasicDataConverter(string path)
        {
            this.path = path;
        }

        ~BasicDataConverter()
        {
            if (deleteTmpFile == true && !string.IsNullOrEmpty(this.path) && File.Exists(this.path))
            {
                File.Delete(this.path);
            }
        }

        #endregion Constructors

        #region IMDataConverter<T> implementation

        public void Convert(T value)
        {
            CheckWriter();
            this.writer.Add(value);
        }

        public void ClearDocument()
        {
            CheckWriter();
            this.writer.Clear();
        }

        public void Flush()
        {
            CheckWriter();
            this.writer.Flush();
        }

        #endregion IMDataConverter<T> implementation

        #region Help methods

        private void CheckWriter()
        {
            if (this.writer == null)
            {
                InitializeWriter();
            }

            if (this.writer == null)
            {
                throw new MultiDocumentException("The writer field is null. Please initialize this field in derived class constructor with valid writer class that implements IMWriter<T> interface");
            }
        }

        protected abstract void InitializeWriter();

        #endregion Help methods
    }
}
