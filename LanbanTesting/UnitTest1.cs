using System;
using System.Diagnostics;
using Lanban.Model;
using Lanban.AccessLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanbanTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var temp = new ProjectAccess().getProjectData(1);
            Debug.Write(temp);
        }
    }
}
