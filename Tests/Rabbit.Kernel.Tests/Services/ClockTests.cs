using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Services;
using System;
using System.Threading;

namespace Rabbit.Kernel.Tests.Services
{
    [TestClass]
    public sealed class ClockTests : TestBase
    {
        #region Property

        public IClock Clock { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void DateTimeAccuracy()
        {
            Assert.AreEqual(DateTime.UtcNow.ToLongDateString(), Clock.UtcNow.ToLongDateString());
        }

        [TestMethod]
        public void DateTimeExpired()
        {
            var token1 = Clock.When(new TimeSpan(0, 0, 1));
            var token2 = Clock.WhenUtc(DateTime.UtcNow.AddSeconds(2));

            Assert.IsTrue(token1.IsCurrent);
            Assert.IsTrue(token2.IsCurrent);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsFalse(token1.IsCurrent);
            Assert.IsTrue(token2.IsCurrent);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsFalse(token2.IsCurrent);
        }

        #endregion Test Method
    }
}