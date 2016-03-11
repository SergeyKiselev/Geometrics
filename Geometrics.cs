using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geometrics
{
    /// <summary>
    /// Интерфейс для реализации геометрических объектов
    /// </summary>
    public interface IGeometrics
    {
        double Area { get; }
    }
    /// <summary>
    /// Базовый класс для подгруппы геометрических объектов "Треугольники"
    /// </summary>
    public class Triangle:IGeometrics
    {
        public virtual double Area { get { return 0; } }
    }
    /// <summary>
    /// Реализация для расчёта площади треугольника по 3 сторонам
    /// </summary>
    public class Triangle3Side : Triangle
    {
        public Triangle3Side(double f_a, double f_b, double f_c)
        {
            a = f_a;
            b = f_b;
            c = f_c;
        }
        public double a { get; set; }
        public double b { get; set; }
        public double c { get; set; }

        public override double Area
        {
            get
            {
                double l_ret = 0;

                double l_p = (a + b + c) / 2; //полупериметр
                double l_v = l_p * (l_p - a) * (l_p - b) * (l_p - c); //площадь по трем сторонам
                double l_s = Math.Sqrt(l_v);
                l_ret = Math.Round(l_s, 2);

                return l_ret;
            }
        }
    }
}
