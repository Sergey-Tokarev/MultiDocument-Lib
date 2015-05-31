using MultiDocument.Readers;
using MultiDocument.Tests.Common.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDocument.Tests.Common
{

    [TestFixture]
    public class MBinaryReaderTest
    {
        [Test]
        [ExpectedException(typeof(MultiDocumentException))]
        public void ConstructorShouldThrowMultiDocumentExceptionTest()
        {
            string path = null; // an invalid path;
            MBinaryReader<Car> reader = new MBinaryReader<Car>(path);
        }

        [Test]
        public void ReadAllTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = string.Empty;

            try
            {
                filePath = TestDataHelper.CreateTempBinaryFile(cars);
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
            finally
            {
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void ReadTest()
        {
            List<Car> cars = TestDataHelper.Cars;
            string filePath = string.Empty;

            try
            {
                filePath = TestDataHelper.CreateTempBinaryFile(cars);
                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);

                Assert.AreEqual(cars.Count, 5);
                Car restoredCar = reader.Read(2); // "Reno logan" 14.07.2013 35000

                Assert.AreEqual(cars[2].Date, restoredCar.Date);
                Assert.AreEqual(cars[2].BrandName, restoredCar.BrandName);
                Assert.AreEqual(cars[2].price, restoredCar.price);
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
                MBinaryReader<Car> reader = new MBinaryReader<Car>(filePath);

                Assert.AreEqual(cars.Count, reader.Count);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
