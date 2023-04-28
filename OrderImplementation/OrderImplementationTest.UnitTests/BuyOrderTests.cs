using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderConsoleApp.Interfaces;
using OrderConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderConsoleAppTests
{
    [TestClass]
    public class BuyOrderTests
    {
        private Mock<IOrderService> _mockOrderService;
       
        [TestInitialize]
        public void SetUp()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockOrderService.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsExceptionIfIOrderServiceParameterIsNull()
        {
            new Order(null, 1.12M);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Threshold cannot be zero")]
        public void ConstructorThrowsExceptionIfThresholdIsDefaultValue()
        {
            new Order(_mockOrderService.Object, 0.00M);
        }

        [TestMethod]
        public void RespondToTickCallsOnThresholdNotExceeded()
        {
            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };          

            var tick = new Tick("TestStock1", 4.99M);

            sut.ProcessTick(tick);

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);

            Assert.AreEqual(OrderStatus.Filled, sut.Status);           
        }

        [TestMethod]
        public void RespondToTickCallsOnThresholdExceeded()
        {
            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };

            var tick = new Tick("TestStock1", 5.78M);

            sut.ProcessTick(tick);

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
        }

        [TestMethod]
        public void RespondToTickCallsOnError()
        {
            _mockOrderService.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>())).Throws<Exception>();

            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };
           
            var tick = new Tick("TestStock1", 4.99M);

            sut.ProcessTick(tick);

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);

            Assert.AreEqual(OrderStatus.Faulted, sut.Status);           
        }

        [TestMethod]
        public void RespondToTickInvokesBuyOnlyWhenOrderStateIsOpen()
        {
            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };

            var tick = new Tick("TestStock1", 4.99M);

            for (int i = 0; i < 10; i++)
            {
                sut.ProcessTick(tick);
            }

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);

            Assert.AreEqual(OrderStatus.Filled, sut.Status);
        }

        [TestMethod]
        public void RespondToTickInvokesBuyOnlyWhenOrderStateIsNotFaulted()
        {
            _mockOrderService.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>())).Throws<Exception>();

            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };          

            var tick = new Tick("TestStock1", 4.99M);

            for (int i = 0; i < 10; i++)
            {
                sut.ProcessTick(tick);
            }

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);

            Assert.AreEqual(OrderStatus.Faulted, sut.Status);
        }

        [TestMethod]
        public async Task RespondToTickInvokesBuyOnlyOnceWhenInvokedByMultipleThreads()
        {
            var sut = new Order(_mockOrderService.Object, 5.77M)
            {
                Quantity = 10,
                Code = "TestStock1"
            };         

            var tick = new Tick("TestStock1", 4.99M);

            var tasks = new List<Task>();

            for (var i = 0; i < 20; i++)
            {
                tasks.Add(Task.Factory.StartNew(() => { sut.ProcessTick(tick); }));
            }

            await Task.WhenAll(tasks);

            _mockOrderService.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);

            Assert.AreEqual(OrderStatus.Filled, sut.Status);
        }
    }
}