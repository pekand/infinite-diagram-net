using Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace TestsBigNumbers
{
    class TestsBigNumbers
    {
        public static void RunTests()
        {

            BigNumber a = null;
            BigNumber b = null;

            /////////////////////////////////////////////////
            a = new BigNumber("-000123000.45600000E-000789000");
            string output = a.ToString();
            Tests.Assert(output == "-123000.456E-789000", "Comparion failed");

        }
    }
}
#endif