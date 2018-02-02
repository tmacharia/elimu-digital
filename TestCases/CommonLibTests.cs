using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCases
{
    [TestClass]
    public class CommonLibTests
    {
        [TestMethod]
        public void FormFileEmptyConstructor()
        {
            IFile file = new FormFile();

            Assert.IsNotNull(file);
            Assert.IsNull(file.Stream);
            Assert.IsNull(file.FileName);
            Assert.AreEqual(0, file.Length);
            Assert.IsNull(file.Name);
        }
    }
}
