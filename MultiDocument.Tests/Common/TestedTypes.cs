using MultiDocument.Common;
using System;
using System.Collections.Generic;


namespace MultiDocument.Tests.Common
{
    public class Car
    {
        #region Constructors

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
    }

    public class NotSupportedType
    {
    }

    public class CarWithNonSupportedTypes
    {
        #region Constructors

        public CarWithNonSupportedTypes()
        {
        }

        public CarWithNonSupportedTypes(NotSupportedType notSupportedType)
        {
            this.NotSupportedTypeProperty = notSupportedType;
        }

        #endregion Constructors

        #region Properties

        [ProcessableAttribute("NotSupportedTypeProperty", Rank = 1)]
        public NotSupportedType NotSupportedTypeProperty { get; set; }

        #endregion Properties
    }
}
