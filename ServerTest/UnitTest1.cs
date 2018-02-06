using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterFeed;
using TwitterFeed.Controllers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ServerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ResponseTest()
        {
            var controller = new HomeController();
            var data = controller.GetFeed();

            Assert.IsTrue(data.IsCompleted);

        }

        [TestMethod]
        public void TweetCountTest()
        {
            var f = new Feed();
            var feed = f.GetTweets("salesforce", 10);
            JArray jdat = JArray.Parse(feed);

            Assert.IsNotNull(feed);
            Assert.AreEqual(10, jdat.Count);
        }
    }
}
