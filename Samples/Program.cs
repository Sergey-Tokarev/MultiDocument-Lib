using MultiDocument;
using MultiDocument.Common;
using MultiDocument.Converters;
using MultiDocument.Interfaces;
using MultiDocument.Readers;
using MultiDocument.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    public class Car
    {
        #region Constructors

        // Please note that a class those properties and fields will be marked with ProcessableAttribute attribute
        // must has a default parameterless constructor
        public Car()
        {
        }

        public Car(DateTime date, string brandName, int price)
        {
            this.Date = date;
            this.BrandName = brandName;
            this.price = price;
        }

        #endregion Constructors

        #region Properties

        [ProcessableAttribute("Date", Rank = 1)]
        public DateTime Date { get; set; }

        [ProcessableAttribute("BrandName", Rank = 2)]
        public string BrandName { get; set; }

        #endregion Properties

        #region Members

        [ProcessableAttribute("Price", Rank = 3)]
        public int price;

        #endregion Members

        #region Methods

        public override string ToString()
        {
            return string.Format("{0,-20}  {1,10}$  {2,15}", BrandName, price, Date);
        }

        #endregion Methods
    }

    class Program
    {
        const string binaryFormat = "binary";
        const string xmlFormat = "xml";

        static Dictionary<string, string> fileNames = new Dictionary<string, string>() 
            {
                { binaryFormat, "cars.bin"},
                { xmlFormat,    "cars.xml"}
            };

        static Dictionary<string, string> convertedFileNames = new Dictionary<string, string>() 
            {
                { binaryFormat, "convertedCars.bin"},
                { xmlFormat,    "convertedCars.xml"}
            };

        static List<Car> cars = new List<Car>() 
            { 
                new Car(new DateTime(2015, 2, 15), "Alpha Romeo Brea", 37000),
                new Car(new DateTime(2014, 3, 16), "BMW", 65000),
                new Car(new DateTime(2013, 4, 17), "Reno logan", 35000),
                new Car(new DateTime(2012, 5, 18), "Peugeot 308", 58000),
                new Car(new DateTime(2011, 6, 19), "Mersedes", 73000)
            };


        static void Main(string[] args)
        {
            try
            {
                DeleteExistingFiles();      // delete all existing files from previouse run

                DoSamplesFor(binaryFormat); // do samples for binary format
                DoSamplesFor(xmlFormat);    // do samples for xml format
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DoSamplesFor(string format)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileNames[format]);

            CreateDocumentSample(path, format, cars);
            OpenExistingDocumentSample(path, format);
            ModifyExistingDocumentSample(path, format);
            RemoveRecordsInExistingDocumentSample(path, format);
            ConvertDocumentSample(path, format);
            WriterConverterSample(path, format);
        }

        static void CreateDocumentSample(string path, string format, List<Car> cars)
        {
            MDocument<Car> doc = new MDocument<Car>(path, format);

            // let's try to add some records

            doc.Add(cars); // add range of records

            Car car = new Car(new DateTime(2015, 1, 14), "Ford Focus", 35000);

            doc.Add(car);  // add a single record
            doc.Flush();   // always flush data to disk when you've finished work with document
        }

        static void OpenExistingDocumentSample(string path, string format)
        {
            MDocument<Car> doc = new MDocument<Car>(path, format);

            // let's try to read all records

            IList<Car> cars = doc.ReadAll();

            Console.WriteLine(string.Format("Document {0} has next records:", path));
            Console.WriteLine();

            foreach (var car in cars)
            {
                Console.WriteLine(car);
            }
        }

        static void ModifyExistingDocumentSample(string path, string format)
        {
            MDocument<Car> doc = new MDocument<Car>(path, format);

            // let's try to read all records

            if (doc.Count > 3)
            {
                // replace the second record (e.g. "BMW") by the third record (e.g. "Reno Logan") in document using index
                doc.Replace(1, doc.Read(2));

                // let's try to replace all records with the same value (e.g. "Reno Logan") by "Alpha Romeo Brea"
                // so we'll have three same records starting from at 0 position

                Car alphaRomeoCar = doc.Read(0);
                Car renoLoganCar = doc.Read(1);
                doc.Replace(renoLoganCar, alphaRomeoCar);
                doc.Flush();

                IList<Car> cars = doc.ReadAll();

                Console.WriteLine();
                Console.WriteLine("Records after replace:");
                Console.WriteLine();

                foreach (var car in cars)
                {
                    Console.WriteLine(car);
                }
            }
        }

        static void RemoveRecordsInExistingDocumentSample(string path, string format)
        {
            MDocument<Car> doc = new MDocument<Car>(path, format);

            // let's try to remove all records (e.g. "Alpha Romeo Brea" cars)

            Car alphaRomeoCar = doc.Read(0);
            doc.Remove(0);             // remove first record by index
            doc.Remove(alphaRomeoCar); // remove the rest two records by value
            doc.Flush();               // note that we don't need to flush data to check absence of specified records,
                                       // but we do this as we know we won't change any data

            IList<Car> cars = doc.ReadAll();

            Console.WriteLine();
            Console.WriteLine("Records after remove:");
            Console.WriteLine();

            foreach (var car in cars)
            {
                Console.WriteLine(car);
            }
        }

        static void ConvertDocumentSample(string path, string format)
        {
            MDocument<Car> doc = new MDocument<Car>(path, format);

            string targetFormat = doc.SupportedFormats.First(e => e != format); // find another supported format
            string convertedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, convertedFileNames[targetFormat]);

            doc.ConvertTo(convertedPath, targetFormat);

            Console.WriteLine(string.Format("Data was successfuly converted from {0} to {1} format.", format, targetFormat));
            Console.WriteLine(string.Format("Converted file path: {0}", convertedPath));

            // let's try to use reader to show how to use them as alternative of using MDocument class

            IMReader<Car> reader = targetFormat == binaryFormat ?
                (IMReader<Car>)new MBinaryReader<Car>(convertedPath) : 
                (IMReader<Car>)new MXmlReader<Car>(convertedPath);

            IList<Car> cars = reader.ReadAll();

            Console.WriteLine();
            Console.WriteLine("Records from converted file:");
            Console.WriteLine();

            foreach (var car in cars)
            {
                Console.WriteLine(car);
            }
        }

        static void WriterConverterSample(string path, string format)
        {
            string targetFormat;
            string convertedPath;

            {
                MDocument<Car> doc = new MDocument<Car>(path, format);

                targetFormat = doc.SupportedFormats.First(e => e != format); // find another supported format
                convertedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, convertedFileNames[targetFormat]);

                if (File.Exists(convertedPath)) // delete file if exist
                {
                    File.Delete(convertedPath);
                }
            }

            // let's try to use writer to show how to use them as alternative of using MDocument class

            IMWriter<Car> writer = format == binaryFormat ?
                (IMWriter<Car>)new MBinaryWriter<Car>(path) : (IMWriter<Car>)new MXmlWriter<Car>(path);

            Car car = new Car(new DateTime(2001, 3, 24), "Honda Civic", 41000); // create new car
            writer.Add(car); // add new car via writer
            writer.Flush();

            // get converter for target format

            IMDataConverter<Car> converter = targetFormat == binaryFormat ?
                (IMDataConverter<Car>)new MBinaryConverter<Car>(convertedPath) :
                (IMDataConverter<Car>)new MXmlConverter<Car>(convertedPath);


            car = new Car(new DateTime(2002, 3, 27), "Mazda", 51000); // create new car
            converter.Convert(car);
            converter.Flush(); // there should be only one record (for "Mazda") in converted file because of we've
                               // specified another path

            IMReader<Car> reader = targetFormat == binaryFormat ?
                (IMReader<Car>)new MBinaryReader<Car>(convertedPath) :
                (IMReader<Car>)new MXmlReader<Car>(convertedPath);

            IList<Car> cars = reader.ReadAll();

            Console.WriteLine();
            Console.WriteLine("Records from converted file via converter:");
            Console.WriteLine();

            foreach (var c in cars)
            {
                Console.WriteLine(c);
            }
        }

        static void DeleteExistingFiles()
        {
            string path = string.Empty;

            // remove all existing files

            foreach (string name in fileNames.Values)
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            foreach (string name in convertedFileNames.Values)
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
