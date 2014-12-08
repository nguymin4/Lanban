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

        [TestMethod]
        public void CheckUsername()
        {
            var temp = new UserAccess();
            var actual = temp.checkUsername("nguymin43");
            //Assert.AreEqual("Existed", actual);
            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void GetFirstUser()
        {
            var temp = new UserAccess();
            var actual = temp.getUserData("nguymin");
            //Assert.AreEqual("Existed", actual);
            System.Diagnostics.Debug.Write(actual.User_ID);
        }
    }
}
