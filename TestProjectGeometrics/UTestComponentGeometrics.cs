using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using G=Geometrics;

namespace TestProjectGeometrics
{
    /// <summary>
    /// Тест расчёта площади треугольника по трём сторонам
    /// </summary>
    [TestClass]
    public class UTestComponentGeometrics
    {
        private G.IGeometrics p_TestGeometrics;
        public UTestComponentGeometrics()
        {
            p_TestGeometrics = new G.Triangle3Side(3, 4, 5);
        }
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestAreaGeometricsCalc()
        {
            Assert.AreEqual(6, p_TestGeometrics.Area);
        }
    }
}
