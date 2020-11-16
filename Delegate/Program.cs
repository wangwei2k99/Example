using System;

namespace Delegate
{
    class Program
    {
        static void Main(string[] args)
        {
            Match match = new Match();
            Func func = new Func(match.Add);
            func += match.Sub;
            func += match.Mul;
            func += match.Div;
            func(2.5, 2);
        }
    }
    delegate double Func(double a, double b);
    class Match
    {
        public double Add(double a, double b) { Console.WriteLine($"{a}+{b}={a + b}"); return a + b; }
        public double Sub(double a, double b) { Console.WriteLine($"{a}-{b}={a - b}"); return a - b; }
        public double Mul(double a, double b) { Console.WriteLine($"{a}*{b}={a * b}"); return a * b; }
        public double Div(double a, double b) { Console.WriteLine($"{a}/{b}={a / b}"); return a / b; }
    }
}
