using MultiDocument.Common;
using MultiDocument.Common.Helpers;
using MultiDocument.Tests.Common;
using MultiDocument.Tests.Common.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Tests
{
    [TestFixture]
    public class BinaryFileRecordParserTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowArgumentNullExceptionTest()
        {
            Stream stream = null;
            BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);
        }

        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorValidationRecordTypesShouldThrowMultiDocumentExceptionTest()
        {
            using (Stream stream = TestDataHelper.CreateBinaryFileStream(new List<Car>()))
            {
                // use CarWithNonSupportedTypes as template parameter even if we create valid stream for Car objects
                BinaryFileRecordParser<CarWithNonSupportedTypes, ProcessableAttribute> parser = new BinaryFileRecordParser<CarWithNonSupportedTypes, ProcessableAttribute>(stream);
            }
        }

        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorStreamLenghtValidationShouldThrowMultiDocumentExceptionTest()
        {
            using (Stream stream = TestDataHelper.CreateBinaryFileStream(new List<Car>()))
            {
                int minStreamLen = TestDataHelper.signature.Length + TestDataHelper.recordsCountSize;
                stream.SetLength(minStreamLen - 1); // set an invalid stream length
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);
            }
        }

        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorSignatureValidationShouldThrowMultiDocumentExceptionTest()
        {
            using (Stream stream = TestDataHelper.CreateBinaryFileStream(new List<Car>()))
            {
                byte[] corruptedSignature = new byte[] { 0x01, 0x02 }; // an invalid signature buffer
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(corruptedSignature, 0, corruptedSignature.Length); // write an invalid signature into stream
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);
            }
        }

        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorNegativeRecordsCountValidationShouldThrowMultiDocumentExceptionTest()
        {
            using (Stream stream = TestDataHelper.CreateBinaryFileStream(new List<Car>()))
            {
                int recordsCount = -2; // set an invalid records count
                byte[] recordsCountBuffer = BitConverter.GetBytes(recordsCount);
                stream.Seek(TestDataHelper.signature.Length, SeekOrigin.Begin);
                stream.Write(recordsCountBuffer, 0, recordsCountBuffer.Length); // write records count
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);
            }
        }

        [Test]
        public void GetAllRecordsTest()
        {
            List<Car> cars = TestDataHelper.Cars;

            using (Stream stream = TestDataHelper.CreateBinaryFileStream(cars))
            {
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);

                List<Car> restoredCars = parser.GetAllRecords();
                Assert.True(cars.Count == restoredCars.Count);

                for (int i = 0; i < restoredCars.Count; ++i)
                {
                    Assert.AreEqual(cars[i].Date, restoredCars[i].Date);
                    Assert.AreEqual(cars[i].BrandName, restoredCars[i].BrandName);
                    Assert.AreEqual(cars[i].price, restoredCars[i].price);
                }
            }
        }

        [Test]
        public void GetRecordTest()
        {
            List<Car> cars = TestDataHelper.Cars;

            using (Stream stream = TestDataHelper.CreateBinaryFileStream(cars))
            {
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);

                Assert.AreEqual(cars.Count, 5);
                Car restoredCar = parser.GetRecord(2); // "Reno logan" 14.07.2013 35000

                Assert.AreEqual(cars[2].Date, restoredCar.Date);
                Assert.AreEqual(cars[2].BrandName, restoredCar.BrandName);
                Assert.AreEqual(cars[2].price, restoredCar.price);
            }
        }

        [Test]
        public void GetRecordsCountTest()
        {
            List<Car> cars = TestDataHelper.Cars;

            using (Stream stream = TestDataHelper.CreateBinaryFileStream(cars))
            {
                BinaryFileRecordParser<Car, ProcessableAttribute> parser = new BinaryFileRecordParser<Car, ProcessableAttribute>(stream);
                Assert.AreEqual(cars.Count, parser.GetRecordsCount());
            }
        }
    }
}
