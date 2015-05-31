using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Tests.Common.Helpers
{
    public class TestDataHelper
    {
        public static byte[] signature = { 0x25, 0x26 };
        public const int recordsCountSize = sizeof(int);
        public static List<string> dates = new List<string>() { "12052011", "13062012", "14072013", "15082014", "16092015" };

        public static List<Car> Cars
        {
            get
            {
                List<DateTime> dtList = new List<DateTime>();

                foreach (string strDate in dates)
                {
                    string format = "ddMMyyyy";
                    DateTime date;

                    if (!DateTime.TryParseExact(strDate, format, null,
                                            DateTimeStyles.AllowWhiteSpaces |
                                            DateTimeStyles.AdjustToUniversal,
                                            out date))
                    {
                        throw new ApplicationException(string.Format("The date {0} cannot be deserialized", strDate));
                    }

                    dtList.Add(date);
                }

                List<Car> cars = new List<Car>() 
                {
                    new Car(dtList[0], "Alpha Romeo Brea", 37000), 
                    new Car(dtList[1], "BMW", 65000), 
                    new Car(dtList[2], "Reno logan", 35000), 
                    new Car(dtList[3], "Peugeot 308", 58000), 
                    new Car(dtList[4], "Mercedes Benz", 73000)
                };

                return cars;
            }
        }

        public static MemoryStream CreateBinaryFileStream(List<Car> cars)
        {
            MemoryStream stream = CreateEmptyBinaryFileStream();
            stream.Position = signature.Length;

            int recordsCount = cars.Count;
            byte[] recordsCountBuffer = BitConverter.GetBytes(recordsCount);
            stream.Write(recordsCountBuffer, 0, recordsCountBuffer.Length); // write records count

            for (int i = 0; i < cars.Count; ++i)
            {
                byte[] strDateBuffer = System.Text.Encoding.ASCII.GetBytes(dates[i]);
                stream.Write(strDateBuffer, 0, strDateBuffer.Length); // write date

                System.Int16 strLength = (System.Int16)cars[i].BrandName.Length;
                byte[] strLengthBuffer = BitConverter.GetBytes(strLength);
                stream.Write(strLengthBuffer, 0, strLengthBuffer.Length); // write string length

                byte[] strBuffer = Encoding.Unicode.GetBytes(cars[i].BrandName);
                stream.Write(strBuffer, 0, strBuffer.Length); // write string

                byte[] priceBuffer = BitConverter.GetBytes(cars[i].price);
                stream.Write(priceBuffer, 0, priceBuffer.Length); // write string length
            }

            return stream;
        }

        public static string CreateTempBinaryFile(List<Car> cars)
        {
            string filePath = Path.GetTempFileName();

            using (Stream stream = CreateBinaryFileStream(cars))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Position = 0;
                    stream.CopyTo(fs);
                }
            }

            return filePath;
        }

        public static MemoryStream CreateEmptyBinaryFileStream()
        {
            MemoryStream stream = new MemoryStream();

            byte[] signature = { 0x25, 0x26 };
            stream.Write(signature, 0, signature.Length); // write signature

            int recordsCount = 0;
            byte[] recordsCountBuffer = BitConverter.GetBytes(recordsCount);
            stream.Write(recordsCountBuffer, 0, recordsCountBuffer.Length); // write records count

            return stream;
        }
    }
}
