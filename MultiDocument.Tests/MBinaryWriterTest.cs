using MultiDocument.Readers;
using MultiDocument.Tests.Common;
using MultiDocument.Tests.Common.Helpers;
using MultiDocument.Writers;
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
    public class MBinaryWriterTest
    {
        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorShouldThrowMultiDocumentExceptionTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = string.Empty;

            try
            {
                filePath = TestDataHelper.CreateTempBinaryFile(cars);
                MBinaryWriter<CarWithNonSupportedTypes> writer = new MBinaryWriter<CarWithNonSupportedTypes>(filePath);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ValidateExistingInvalidFileShouldThrowMultiDocumentExceptionTest()
        {
            string filePath = string.Empty;

            try
            {
                filePath = TestDataHelper.CreateTempBinaryFile(new List<Car>());
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fs.Position = TestDataHelper.signature.Length;
                    int recordsCount = 10; // set invalid records count to corrupt file
                    byte[] recordsCountBuffer = BitConverter.GetBytes(recordsCount);
                    fs.Write(recordsCountBuffer, 0, recordsCountBuffer.Length); // write records count
                }

                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void AddTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                
                foreach(Car car in cars)
                {
                    writer.Add(car);
                }

                CheckContent(writer, filePath, cars);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void AddCollectionOfCarsTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                CheckContent(writer, filePath, cars);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void ReplaceRecordAtPositionTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Replace(1, cars[0]);
                writer.Flush();

                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);
                Car firstCar = reader.Read(0); // read first record
                Car secondCar = reader.Read(1); // read first record
                Assert.True(cars.Count == reader.Count);

                Assert.AreEqual(firstCar.Date, secondCar.Date);
                Assert.AreEqual(firstCar.BrandName, secondCar.BrandName);
                Assert.AreEqual(firstCar.price, secondCar.price);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void ReplaceRecordByValueTest()
        {                                         //        0             1        2            3             4
            List<Car> cars = TestDataHelper.Cars; // "Alpha Romeo Brea" "BMW" "Reno logan" "Peugeot 308" "Mercedes Benz"
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Replace(3, cars[1]); // first at all try to replace "Peugeot 308" by "BMW" via position
                writer.Flush();

                Car carAlphaRomeo = cars[0];
                Car carBMW = cars[1];

                // Try to replace all occurence of "BMW" car by "Alpha Romeo Brea", so we'll have the result like this
                //         0                  1               2               3                4
                // "Alpha Romeo Brea" "Alpha Romeo Brea" "Reno logan" "Alpha Romeo Brea" "Mercedes Benz"

                writer.Replace(carBMW, carAlphaRomeo);
                writer.Flush();

                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);
                List<Car> restoredCars = (List<Car>)reader.ReadAll(); // read all records

                int count = restoredCars.Count(c => c.BrandName == carAlphaRomeo.BrandName && 
                                                    c.Date == carAlphaRomeo.Date && 
                                                    c.price == carAlphaRomeo.price);
                Assert.AreEqual(count, 3);

            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAtNegativePositionShouldThrowArgumentOutOfRangeExceptionTest()
        {                                         
            List<Car> cars = TestDataHelper.Cars;
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Remove(-2); // try to remove a record at invalid position
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAfterEndOfRangePositionShouldThrowArgumentOutOfRangeExceptionTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Remove(cars.Count + 2); // try to remove a record at position greater than records count
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void RemoveRecordAtPositionTest()
        {                                             //        0             1        2            3             4
            List<Car> cars = TestDataHelper.Cars;     // "Alpha Romeo Brea" "BMW" "Reno logan" "Peugeot 308" "Mercedes Benz"
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Remove(3); // remove "Peugeot 308"
                writer.Flush();

                Car carPeugeot = cars[3];

                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);
                List<Car> restoredCars = (List<Car>)reader.ReadAll(); // read all records

                int count = restoredCars.Count(c => c.BrandName == carPeugeot.BrandName &&
                                                    c.Date == carPeugeot.Date &&
                                                    c.price == carPeugeot.price);
                Assert.AreEqual(count, 0);

            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void RemoveRecordByValueTest()
        {                                             //        0             1        2            3             4
            List<Car> cars = TestDataHelper.Cars;     // "Alpha Romeo Brea" "BMW" "Reno logan" "Peugeot 308" "Mercedes Benz"
            string filePath = Path.GetTempFileName();

            try
            {
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);
                writer.Add(cars); // add a collection of cars
                writer.Replace(3, cars[1]); // first at all try to replace "Peugeot 308" by "BMW" via position
                writer.Flush();

                Car carAlphaRomeo = cars[0];
                Car carBMW = cars[1];

                // Try to replace all occurence of "BMW" car by "Alpha Romeo Brea", so we'll have the result like this
                //         0                  1               2               3                4
                // "Alpha Romeo Brea" "Alpha Romeo Brea" "Reno logan" "Alpha Romeo Brea" "Mercedes Benz"

                writer.Replace(carBMW, carAlphaRomeo);
                writer.Flush();

                writer.Remove(carAlphaRomeo);
                writer.Flush();

                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);
                List<Car> restoredCars = (List<Car>)reader.ReadAll(); // read all records

                int count = restoredCars.Count(c => c.BrandName == carAlphaRomeo.BrandName &&
                                                    c.Date == carAlphaRomeo.Date &&
                                                    c.price == carAlphaRomeo.price);
                Assert.AreEqual(count, 0);

            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void CountTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = string.Empty;

            try
            {
                filePath = TestDataHelper.CreateTempBinaryFile(cars);
                MBinaryWriter<Car> writer = new MBinaryWriter<Car>(filePath);

                Assert.AreEqual(cars.Count, writer.Count);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        #region Help methods

        private void CheckContent(MBinaryWriter<Car> writer, string filePath, List<Car> cars)
        {
            Assert.True(File.ReadAllBytes(filePath).Length == 0); // we haven't flushed data, so file should be empty
            Assert.AreEqual(cars.Count, writer.Count);

            writer.Flush(); // flush content to file so we can try to read flushed content via MBinaryReader

            MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);
            List<Car> restoredCars = (List<Car>)reader.ReadAll(); // read all records using MBinaryReader
            Assert.True(cars.Count == restoredCars.Count);

            for (int i = 0; i < restoredCars.Count; ++i)
            {
                Assert.AreEqual(cars[i].Date, restoredCars[i].Date);
                Assert.AreEqual(cars[i].BrandName, restoredCars[i].BrandName);
                Assert.AreEqual(cars[i].price, restoredCars[i].price);
            }
        }

        #endregion Help methods
    }
}
