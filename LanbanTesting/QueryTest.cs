using System;
using System.Diagnostics;
using Lanban.Model;
using Lanban.AccessLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanbanTesting
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void Test()
        {
            var temp = new ProjectAccess();
            var actual = temp.IsOwner(1, 1);
            Assert.AreEqual(true, actual);
        }
    }
}
