using Diagram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace TestsBigNumbers
{
    class TestsBigInteger
    {
        public static void RunTests()
        {
            string output = "";
            BigInteger a = null;
            BigInteger b = null;

            /////////////////////////////////////////////////

            a = new BigInteger("1234");
            b = new BigInteger("1234");
            Tests.Assert(a.Compare(b) == 0, "Comparion failed");

            a = new BigInteger("1234");
            b = new BigInteger("1200");
            Tests.Assert(a.Compare(b) == 1, "Comparion failed");

            a = new BigInteger("1200");
            b = new BigInteger("1234");
            Tests.Assert(a.Compare(b) == -1, "Comparion failed");

            a = new BigInteger("123456");
            b = new BigInteger("1234");
            Tests.Assert(a.Compare(b) == 1, "Comparion failed");

            a = new BigInteger("123456");
            b = new BigInteger("-1234");
            Tests.Assert(a.Compare(b) == 1, "Comparion failed");

            a = new BigInteger("-123456");
            b = new BigInteger("1234");
            Tests.Assert(a.Compare(b) == -1, "Comparion failed");

            a = new BigInteger("-123456");
            b = new BigInteger("-1234");
            Tests.Assert(a.Compare(b) == -1, "Comparion failed");

            /////////////////////////////////////////////////


            a = new BigInteger("1234");
            b = new BigInteger("1234");
            Tests.Assert(a.IsEqual(b) == true, "Comparion failed");

            a = new BigInteger("1234");
            b = new BigInteger("1234567");
            Tests.Assert(a.IsSmaller(b) == true, "Comparion failed");

            a = new BigInteger("1234567");
            b = new BigInteger("1234");
            Tests.Assert(a.IsGreater(b) == true, "Comparion failed");

            a = new BigInteger("1234");
            b = new BigInteger("1234567");
            Tests.Assert(a.IsSmallerOrEqual(b) == true, "Comparion failed");

            a = new BigInteger("1234567");
            b = new BigInteger("1234");
            Tests.Assert(a.IsGreaterOrEqual(b) == true, "Comparion failed");


            /////////////////////////////////////////////////

            a = new BigInteger("10");
            Tests.Assert(a.IsInt() == true, "Check type failed");

            a = new BigInteger("10000000000001000000000000000");
            Tests.Assert(a.IsInt() == false, "Check type failed");

            a = new BigInteger("-10000000000001000000000000000");
            Tests.Assert(a.IsInt() == false, "Check type failed");

            a = new BigInteger("1231231");
            Tests.Assert(a.AsInt() == 1231231, "Convert to int failed");

            a = new BigInteger("-1231231");
            Tests.Assert(a.AsInt() == -1231231, "Convert to int failed");

            a = new BigInteger("1231231");
            Tests.Assert(a.AsLong() == 1231231, "Convert to long failed");

            a = new BigInteger("-1231231");
            Tests.Assert(a.AsLong() == -1231231, "Convert to long failed");

            /////////////////////////////////////////////////

            a = new BigInteger("10000000000001000000000000000");
            b = new BigInteger("1000");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "10000000000001000000000001000", "Addition failed");

            a = new BigInteger("10");
            b = new BigInteger("5");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "15", "Addition failed");

            a = new BigInteger("5");
            b = new BigInteger("10");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "15", "Addition failed");

            a = new BigInteger("-5");
            b = new BigInteger("10");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "5", "Addition failed");

            a = new BigInteger("5");
            b = new BigInteger("-10");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "-5", "Addition failed");

            a = new BigInteger("-5");
            b = new BigInteger("-10");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "-15", "Addition failed");

            a = new BigInteger("-5");
            b = new BigInteger("5");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "0", "Addition failed");

            a = new BigInteger("5");
            b = new BigInteger("-5");
            output = a.Addition(b).ToString();
            Tests.Assert(output == "0", "Addition failed");

            /////////////////////////////////////////////////

            a = new BigInteger("10");
            b = new BigInteger("5");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "5", "Subtraction failed");

            a = new BigInteger("5");
            b = new BigInteger("10");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "-5", "Subtraction failed");

            a = new BigInteger("-5");
            b = new BigInteger("10");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "-15", "Subtraction failed");

            a = new BigInteger("5");
            b = new BigInteger("-10");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "15", "Subtraction failed");

            a = new BigInteger("-5");
            b = new BigInteger("-10");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "5", "Subtraction failed");

            a = new BigInteger("5");
            b = new BigInteger("5");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "0", "Subtraction failed");

            a = new BigInteger("-5");
            b = new BigInteger("-5");
            output = a.Subtraction(b).ToString();
            Tests.Assert(output == "0", "Subtraction failed");

            /////////////////////////////////////////////////

            a = new BigInteger("10");
            b = new BigInteger("5");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "50", "Multiplication failed");

            a = new BigInteger("5");
            b = new BigInteger("10");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "50", "Multiplication failed");

            a = new BigInteger("-5");
            b = new BigInteger("10");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "-50", "Multiplication failed");

            a = new BigInteger("10");
            b = new BigInteger("-5");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "-50", "Multiplication failed");

            a = new BigInteger("5");
            b = new BigInteger("-10");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "-50", "Multiplication failed");

            a = new BigInteger("-10");
            b = new BigInteger("5");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "-50", "Multiplication failed");

            a = new BigInteger("-5");
            b = new BigInteger("-10");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "50", "Multiplication failed");

            a = new BigInteger("-10");
            b = new BigInteger("-5");
            output = a.Multiplication(b).ToString();
            Tests.Assert(output == "50", "Multiplication failed");

            /////////////////////////////////////////////////

            a = new BigInteger("11025");
            b = new BigInteger("105");
            output = a.Division(b).ToString();
            Tests.Assert(output == "105|0", "Division failed");

            a = new BigInteger("15");
            b = new BigInteger("5");
            output = a.Division(b).ToString();
            Tests.Assert(output == "3|0", "Division failed");

            a = new BigInteger("42435");
            b = new BigInteger("123");
            output = a.Division(b).ToString();
            Tests.Assert(output == "345|0", "Division failed");

            a = new BigInteger("-42435");
            b = new BigInteger("123");
            output = a.Division(b).ToString();
            Tests.Assert(output == "-345|0", "Division failed");

            a = new BigInteger("42435");
            b = new BigInteger("-123");
            output = a.Division(b).ToString();
            Tests.Assert(output == "-345|0", "Division failed");

            a = new BigInteger("-42435");
            b = new BigInteger("-123");
            output = a.Division(b).ToString();
            Tests.Assert(output == "345|0", "Division failed");

            a = new BigInteger("42438");
            b = new BigInteger("123");
            output = a.Division(b).ToString();
            Tests.Assert(output == "345|3", "Division failed");

            a = new BigInteger("214312342134123412341234");
            b = new BigInteger("21312432134");
            output = a.Division(b).ToString();
            Tests.Assert(output == "10055743088665|11055180124", "Division failed");

            a = new BigInteger("42438");
            b = new BigInteger("123");
            output = a.Modulo(b).ToString();
            Tests.Assert(output == "3", "Division failed");

            a = new BigInteger("42557");
            b = new BigInteger("123");
            output = a.Modulo(b).ToString();
            Tests.Assert(output == "122", "Division failed");

        }

        public static void RunSpeedTests()
        {
            BigInteger a = null;
            BigInteger b = null;

            //speed test
            Random rnd = new Random();
            string output = "";
            string output2 = "";

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 99999; i++)
            {
                int operation = rnd.Next(1, 5);

                long c1 = rnd.Next(int.MinValue, int.MaxValue);
                long c2 = rnd.Next(int.MinValue, int.MaxValue);

                a = new BigInteger(c1.ToString());
                b = new BigInteger(c2.ToString());

                if (operation == 1)
                {
                    output = a.Addition(b).ToString();
                    output2 = (c1 + c2).ToString();
                    Tests.Assert(output == output2, "SpeedTest failed");
                }

                if (operation == 2)
                {
                    output = a.Subtraction(b).ToString();
                    output2 = (c1 - c2).ToString();
                    Tests.Assert(output == output2, "SpeedTest failed");
                }

                if (operation == 3)
                {
                    output = a.Multiplication(b).ToString();
                    output2 = (c1 * c2).ToString();
                    Tests.Assert(output == output2, "SpeedTest failed");
                }

                if (operation == 4)
                {
                    output = a.Division(b).Division().ToString();
                    output2 = (c1 / c2).ToString();
                    Tests.Assert(output == output2, "SpeedTest failed");
                }
            }

            sw.Stop();
            Tests.Assert(sw.Elapsed.Seconds <= 2, "speed test take to long");
        }
    }
}
#endif