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
    public class BinaryHelperTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteRecordsShouldThrowArgumentNullExceptionTest()
        {
            Stream stream = null;
            List<Car> cars = TestDataHelper.Cars;
            BinaryHelper<Car, ProcessableAttribute>.WriteRecords(cars, stream);

        }

        [Test]
        public void WriteRecordsTest()
        {
            List<Car> cars = TestDataHelper.Cars;

            using(Stream stream = TestDataHelper.CreateEmptyBinaryFileStream())
            {
                using(Stream comparedStream = TestDataHelper.CreateBinaryFileStream(cars))
                {
                    BinaryHelper<Car, ProcessableAttribute>.WriteRecords(cars, stream);
                    byte[] buffer1 = new byte[stream.Length];
                    byte[] buffer2 = new byte[comparedStream.Length];

                    stream.Position = 0;
                    comparedStream.Position = 0;
                    stream.Read(buffer1, 0, buffer1.Length);
                    comparedStream.Read(buffer2, 0, buffer2.Length);

                    Assert.True(OperationsHelper.CompareByteArrays(buffer1, buffer2));
                }
            }
        }
    }
}
