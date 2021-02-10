using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace TestsBigNumbers
{
    class Tests
    {
        public static void Assert(bool state, string message)
        {
            if (!state) {
                throw new Exception(message);
            }
        }

        public static void RunTests() {
            TestsBigInteger.RunTests();
            TestsBigInteger.RunSpeedTests();
            TestsBigNumbers.RunTests();
        }
    }
}
#endif