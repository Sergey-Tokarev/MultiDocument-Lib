using MultiDocument.Interfaces;
using System;
using System.Globalization;
using System.Text;

namespace MultiDocument.Serializers
{
    public class DateSerializer : IDataSerializer
    {
        #region IDataSerializer implementation

        public byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();

            if (type != typeof(System.DateTime))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be serialized", type));
            }

            DateTime date = (DateTime)obj;
            string strDate = date.ToString("ddMMyyyy");
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(strDate);

            return buffer;
        }

        public object Deserialize(byte[] buffer, Type type)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (type != typeof(System.DateTime))
            {
                throw new MultiDocumentException(string.Format("The type {0} cannot be deserialized", type));
            }

            string strDate = Encoding.ASCII.GetString(buffer);
            string format = "ddMMyyyy";
            DateTime date;

            if (!DateTime.TryParseExact(strDate, format, null,
                                    DateTimeStyles.AllowWhiteSpaces |
                                    DateTimeStyles.AdjustToUniversal,
                                    out date))
            {
                throw new MultiDocumentException(string.Format("The date {0} cannot be deserialized", strDate));
            }

            return date;
        }

        #endregion IDataSerializer implementation
    }
}
