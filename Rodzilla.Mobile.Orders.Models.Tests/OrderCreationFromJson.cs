using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rodzilla.Mobile.Orders.Models.Tests
{
    [TestClass]
    public class OrderCreationFromJson
    {
        private Order _order;

        [TestInitialize]
        public void InitTest()
        {
            const string json = @"{""CustomerId"":""7aee62db-b573-4c8e-bc52-4c3469266197"",""CustomerFirstName"":""Roland"",""CustomerPhone"":""+12063832022"",""Price"":2000,""AppFee"":300,""QuotedPrice"":2300,""EstimatedMinutes"":10,""PointsValue"":23,""StartTime"":null,""FinishTime"":null,""OriginalOrder"":{""Message"":""2 brekkies and two large americanos thx"",""Sent"":""2019-05-04T16:01:27.6455713-07:00""},""Details"":[{""$type"":""Rodzilla.Mobile.Orders.Models.OrderQuestion, Rodzilla.Mobile.Orders.Models"",""Message"":""Do you want room for cream in your americanos?"",""Sent"":""2019-05-04T16:01:27.6461842-07:00""},{""$type"":""Rodzilla.Mobile.Orders.Models.OrderResponse, Rodzilla.Mobile.Orders.Models"",""Message"":""Yes please"",""Sent"":""2019-05-04T16:01:27.6463374-07:00""}],""Id"":""d574f5f5-0e6d-41ff-bf29-f725ac188ebb""}";

            _order = OrderManager.FromJson(json);
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
            //OrderManager.Ask("Do you want room for cream in your americanos?", ref _order);

            Assert.IsTrue(_order.Details.Count > 0);
            //OrderManager.Reply("Yes please", ref _order);

            Assert.IsTrue(_order.Details.Count > 1);

            //OrderManager.EstimateOrder(1500, 10, ref _order);
            Assert.IsTrue(_order.Price > 0);

            Assert.AreEqual(46, _order.PointsValue);
        }
    }
}
