using Common.ViewModels;
using DAL.Models;
using Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCases
{
    [TestClass]
    public class ExtensionsTestCases
    {
        [TestMethod]
        public void UpdateReflector()
        {
            School school = new School()
            {
                Name = "JkUAt",
                ViceChancellor = "Prof Mabel Imbuga",
                Website = "www.jkuat.ac.ke",
                DateFounded = DateTime.Parse("07-20-1956"),
            };
            SchoolViewModel model = new SchoolViewModel()
            {
                Name = "JKUAT",
                ViceChancellor = "Mabel Imbuga",
                Website = "http://www.jkuat.ac.ke",
                DateFounded = DateTime.Parse("07-20-1956"),
            };

            var result = school.UpdateReflector(model);

            Assert.AreEqual(3, result.TotalUpdates);
        }
    }
}
