using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCases
{
    [TestClass]
    public class TestCases
    {
        [TestMethod]
        public void TestMethod()
        {
            double answer = 109.64;
            double result = Global.Add(25, 28.45, 56.19);

            Assert.AreEqual(answer, result);
        }
    }

    public static class Global
    {
        public static double Add(params double[] numbers)
        {
            double total = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                total += numbers[i];
            }

            return total;
        }
    }
}
