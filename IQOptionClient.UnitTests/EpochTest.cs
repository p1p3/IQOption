using System;
using System.Globalization;
using IQOptionClient.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IQOptionClient.UnitTests
{
    [TestClass]
    public class EpochTest
    {
        [TestMethod]
        public void NanoSecondsUnixToDateTimeTestFrom()
        {
            //Arrange
            long unixTime = 1521999070048050813;
            DateTime expectedTime = new DateTime(2018,3,25,17,31,10);
            var epoch = new Epoch();

            //Act
            var time = epoch.FromUnixTimeToDateTime(unixTime);

            //Assert
            Assert.AreEqual(expectedTime.ToString(CultureInfo.InvariantCulture),time.ToString(CultureInfo.InvariantCulture));
        }


        [TestMethod]
        public void SecondsUnixToDateTimeTestFrom()
        {
            //Arrange
            long unixTime = 1522004766;
            DateTime expectedTime = new DateTime(2018, 3, 25, 19, 06, 06);
            var epoch = new Epoch();

            //Act
            var time = epoch.FromUnixTimeToDateTime(unixTime);

            //Assert
            Assert.AreEqual(expectedTime.ToString(CultureInfo.InvariantCulture), time.ToString(CultureInfo.InvariantCulture));
        }
    }
}
