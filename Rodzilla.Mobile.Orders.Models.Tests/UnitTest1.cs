using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rodzilla.Mobile.Orders.Models.Tests
{
    [TestClass]
    public class OrderCreationToJson
    {
        private Order _order;

        [TestInitialize]
        public void InitTest()
        {
            var customer = new Customer() { FirstName = "Roland", Id = "+12063832022" };
            _order = OrderManager.NewOrder(customer);
            _order.OriginalOrder = new OrderMessage
            {
                Message = "2 brekkies and two large americanos thx", Sent = DateTime.Now
            };
        }
        [TestCleanup]
        public void TestSerialize()
        {
            Console.Write(OrderManager.Serialize(_order));
        }
        [TestMethod]
        public void TestQuestion()
        {
            Assert.IsNotNull(_order.Id);
            OrderManager.Ask("Do you want room for cream in your americanos?", ref _order);

            Assert.IsTrue(_order.Details.Count > 0);
            OrderManager.Reply("Yes please", ref _order);

            Assert.IsTrue(_order.Details.Count > 1);

            OrderManager.EstimateOrder(2000, 10, ref _order);
            Assert.IsTrue(_order.Price > 0);
            Assert.AreEqual("Roland", _order.CustomerFirstName);
            Assert.AreEqual(46, _order.PointsValue);
        }
    }
}
