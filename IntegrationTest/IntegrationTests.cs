using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Bots.BotTypes.Class;
using Model.Bots.UnitTestBot;

namespace IntegrationTest
{
    [TestClass]
    public class IntegrationTests
    {
        private readonly SimpleMsgBot _convert = new SimpleMsgBot();

        [TestMethod]
        public void TestMethod1()
        {
            var list = _convert.GetMessages("/start");

            Assert.IsNotNull(list);
            Assert.AreNotEqual(list.Count, 0);
        }
    }
}
