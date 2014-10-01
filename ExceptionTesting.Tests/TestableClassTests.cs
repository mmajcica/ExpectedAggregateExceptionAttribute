using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExceptionTesting.Tests
{
    [TestClass]
    public class TestableClassTests
    {
        private TestableClass sut;

        [TestInitialize]
        public void TestInitialize()
        {
            sut = new TestableClass();    
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MySimpleMethod_Throws_ArgumentNullException()
        {
            sut.MySimpleMethod(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MySimpleMethodAsync_Throws_ArgumentNullException()
        {
            sut.MySimpleMethodAsync(null).Wait();
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(ArgumentNullException))]
        public void MySimpleMethodAsync_Throws_ArgumentNullException_2()
        {
            sut.MySimpleMethodAsync(null).Wait();
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(ArgumentNullException))]
        public void ArgumentNullException_Fail_If_No_Exception()
        {
            sut.MySimpleMethodAsync(new List<string>()).Wait();
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(ArgumentNullException))]
        public void ArgumentNullException_Fail_If_Wrong_Inner_Exception()
        {
            Task.Factory.StartNew(() =>
            {
                throw new ArgumentOutOfRangeException();
            });
        }
    }
}
