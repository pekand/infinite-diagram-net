using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diagram
{
    public class InvalidBigNumber : Exception
    {
        public InvalidBigNumber() : base() { }
        public InvalidBigNumber(string message) : base(message) { }
        public InvalidBigNumber(string message, Exception inner) : base(message, inner) { }
    }

    public class BigNumberDivisionResult
    {
        private BigNumber division = null;
        private BigNumber reminder = null;

        public BigNumberDivisionResult(BigNumber division, BigNumber reminder)
        {
            this.division = new BigNumber(division);
            this.reminder = new BigNumber(reminder);
        }

        public BigNumber Division()
        {
            return new BigNumber(division);
        }

        public BigNumber Reminder()
        {
            return new BigNumber(reminder);
        }

        public override string ToString()
        {
            string result = "";

            if (division != null)
            {
                result += this.division.ToString();
            }

            if (reminder != null)
            {
                result += "|";
                result += this.reminder.ToString();
            }

            return result;
        }
    }

    public class BigNumber
    {
        public const bool POSITIVE = true;
        public const bool NEGATIVE = false;


        private bool orientation = true;
        private List<byte> digitsBeforeDecimalPoint = null;
        private List<byte> digitsAfterDecimalPoint = null;
        private bool exponentOrientation = true;
        private List<byte> exponentDigits = null;

        public BigNumber(string number = "0")
        {
            this.validate(number);
        }

        public BigNumber(bool orientation, List<byte> digits, bool inverse = false)
        {
            this.orientation = orientation;

            if (digits != null && digits.Count > 0)
            {
                this.digitsBeforeDecimalPoint = new List<byte>();


                if (inverse)
                {
                    for (int i = digits.Count - 1; i >= 0; i--)
                    {
                        this.digitsBeforeDecimalPoint.Add(digits[i]);
                    }
                }
                else
                {
                    foreach (byte digit in digits)
                    {
                        this.digitsBeforeDecimalPoint.Add(digit);
                    }
                }
            }
        }

        public BigNumber(BigNumber num, bool absolute = false)
        {
            if (num != null)
            {
                this.orientation = absolute ? true : num.orientation;

                this.digitsBeforeDecimalPoint = new List<byte>();

                for (int i = 0; i < num.Length(); i++)
                {
                    this.digitsBeforeDecimalPoint.Add(num.get(i));
                }
            }
        }

        public bool validate(string number)
        {
            Match m = Regex.Match(number, @"\s*(-|\+)?0*(\d*)\.?(\d*)?(E|e)?(-|\+)?0*(\d*)\s*", RegexOptions.IgnoreCase);
            if (m.Success)
            {

                this.orientation = m.Groups[1].Captures.Count > 0 && m.Groups[1].Captures[0].Value == "-" ? false : true;

                this.digitsBeforeDecimalPoint = m.Groups[2].Captures.Count > 0 ? this.ConvertToByteArrayInverse(m.Groups[2].Captures[0].Value) : null;
                this.digitsAfterDecimalPoint = m.Groups[3].Captures.Count > 0 ?  this.ConvertToByteArrayInverse(m.Groups[3].Captures[0].Value) : null;

                this.exponentOrientation = m.Groups[5].Captures.Count > 0 && m.Groups[5].Captures[0].Value == "-"  ? false: true;
                this.exponentDigits = m.Groups[6].Captures.Count > 0 ? this.ConvertToByteArrayInverse(m.Groups[6].Captures[0].Value) : null;

                return true;
            }
            else
            {
                throw new InvalidBigNumber("Invalid format");
            }

            return false;
        }

        public byte get(int i)
        {
            if (digitsBeforeDecimalPoint != null && i < this.digitsBeforeDecimalPoint.Count)
            {
                return this.digitsBeforeDecimalPoint[i];
            }

            return 0;
        }

        private List<byte> ConvertToByteArray(string num)
        {
            List<byte> number = new List<byte>();

            for (int i = 0; i < num.Length; i++)
            {
                number.Add((byte)(num[i] - '0'));
            }

            return number;
        }

        private List<byte> ConvertToByteArrayInverse(string num)
        {
            List<byte> number = new List<byte>();

            for (int i = num.Length - 1; i >= 0; i--)
            {
                number.Add((byte)(num[i] - '0'));
            }

            return number;
        }

        public int Length()
        {
            return this.digitsBeforeDecimalPoint == null ? 0 : this.digitsBeforeDecimalPoint.Count();
        }

        public bool IsPositive()
        {
            return this.orientation;
        }

        public bool IsNegative()
        {
            return !this.orientation;
        }

        public BigNumber Absolute()
        {
            return new BigNumber(this, true);
        }

        public bool IsEqual(BigNumber num, bool skipOrientation = false)
        {
            return this.Compare(num, skipOrientation) == 0;
        }

        public bool IsGreater(BigNumber num, bool skipOrientation = false)
        {
            return this.Compare(num, skipOrientation) == 1;
        }

        public bool IsGreaterOrEqual(BigNumber num, bool skipOrientation = false)
        {
            return this.Compare(num, skipOrientation) >= 0;
        }

        public bool IsSmaller(BigNumber num, bool skipOrientation = false)
        {
            return this.Compare(num, skipOrientation) == -1;
        }
        public bool IsSmallerOrEqual(BigNumber num, bool skipOrientation = false)
        {
            return this.Compare(num, skipOrientation) <= 0;
        }

        public int Compare(BigNumber num, bool skipOrientation = false)
        {

            if (!skipOrientation)
            {
                if (this.IsPositive() && num.IsNegative())
                {
                    return 1;
                }

                if (this.IsNegative() && num.IsPositive())
                {
                    return -1;
                }
            }

            int alen = this.Length();
            int blen = num.Length();

            int len = alen > blen ? alen : blen;

            for (int i = len - 1; i >= 0; i--)
            {
                if (this.get(i) > num.get(i))
                {

                    if (skipOrientation || this.IsPositive())
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                if (this.get(i) < num.get(i))
                {
                    if (skipOrientation || this.IsPositive())
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        public BigNumber Addition(BigNumber num)
        {
            bool resultOrientation = true;
            bool switchOrder = false;
            bool addition = true;


            if (this.IsPositive() && num.IsPositive())
            {
                addition = true;
                resultOrientation = POSITIVE;
            }
            else
            if (this.IsPositive() && num.IsNegative())
            {
                addition = false;
                resultOrientation = POSITIVE;

                if (this.Compare(num, true) < 0)
                {
                    resultOrientation = NEGATIVE;
                    switchOrder = true;
                }

            }
            else
            if (this.IsNegative() && num.IsPositive())
            {
                addition = false;
                resultOrientation = NEGATIVE;

                if (this.Compare(num, true) < 0)
                {
                    resultOrientation = POSITIVE;
                    switchOrder = true;
                }

            }
            else
            if (this.IsNegative() && num.IsNegative())
            {
                addition = true;
                resultOrientation = NEGATIVE;
            }


            int alen = this.Length();
            int blen = num.Length();

            int len = alen > blen ? alen : blen;

            int t = 0;

            List<byte> output = new List<byte>();

            for (int i = 0; i < len; i++)
            {
                int c1 = this.get(i);
                int c2 = num.get(i);
                int n = 0;

                if (addition)
                {
                    n = c1 + c2 + t;

                    t = 0;
                    if (n > 9)
                    {
                        t = n / 10;
                    }

                    output.Add((byte)(n % 10));
                }
                else
                {
                    if (switchOrder)
                    {
                        n = c2 - (c1 + t);
                    }
                    else
                    {
                        n = c1 - (c2 + t);
                    }

                    t = 0;
                    if (n < 0)
                    {
                        n = 10 + n;
                        t = 1;
                    }
                    output.Add((byte)(n));
                }

            }

            if (t > 0)
            {
                while (t > 0)
                {
                    output.Add((byte)(t % 10));
                    t = t / 10;
                }
            }

            BigNumber test = new BigNumber(resultOrientation, output);

            return test;
        }

        public BigNumber Subtraction(BigNumber num)
        {
            bool resultOrientation = true;
            bool switchOrder = false;
            bool subtraction = true;

            if (this.Compare(num, true) >= 0)
            {
                if (this.IsPositive() && num.IsPositive())
                {
                    subtraction = true;
                    resultOrientation = POSITIVE;
                }

                if (this.IsPositive() && num.IsNegative())
                {
                    subtraction = false;
                    resultOrientation = POSITIVE;
                }

                if (this.IsNegative() && num.IsPositive())
                {
                    subtraction = false;
                    resultOrientation = NEGATIVE;
                }

                if (this.IsNegative() && num.IsNegative())
                {
                    subtraction = true;
                    resultOrientation = NEGATIVE;
                }

            }
            else
            {

                switchOrder = true;

                if (this.IsPositive() && num.IsPositive())
                {
                    subtraction = true;
                    resultOrientation = NEGATIVE;
                }

                if (this.IsPositive() && num.IsNegative())
                {
                    subtraction = false;
                    resultOrientation = POSITIVE;
                }

                if (this.IsNegative() && num.IsPositive())
                {
                    subtraction = false;
                    resultOrientation = NEGATIVE;
                }

                if (this.IsNegative() && num.IsNegative())
                {
                    subtraction = true;
                    resultOrientation = POSITIVE;
                }
            }


            int alen = this.Length();
            int blen = num.Length();

            int len = alen > blen ? alen : blen;

            int t = 0;

            List<byte> output = new List<byte>();

            for (int i = 0; i < len; i++)
            {
                int c1 = this.get(i);
                int c2 = num.get(i);

                int n = 0;

                if (subtraction)
                {
                    if (switchOrder)
                    {
                        n = c2 - (c1 + t);
                    }
                    else
                    {
                        n = c1 - (c2 + t);
                    }

                    t = 0;
                    if (n < 0)
                    {
                        n = 10 + n;
                        t = 1;
                    }
                    output.Add((byte)(n));
                }
                else
                {
                    n = c1 + c2 + t;

                    t = 0;
                    if (n > 9)
                    {
                        t = n / 10;
                    }

                    output.Add((byte)(n % 10));
                }
            }

            return new BigNumber(resultOrientation, output);
        }

        public BigNumber Multiplication(BigNumber num)
        {
            int alen = this.Length();
            int blen = num.Length();

            List<List<byte>> rows = new List<List<byte>>();

            List<byte> row = new List<byte>();

            int t = 0;
            int space = 0;
            int maxlen = 0;

            for (int i = 0; i < blen; i++)
            {
                int c2 = num.get(i);

                row = new List<byte>();

                t = 0;

                for (int k = 0; k < space; k++)
                {
                    row.Add(0);

                }

                for (int j = 0; j < alen; j++)
                {
                    int c1 = this.get(j);

                    int n = c2 * c1 + t;

                    row.Add((byte)(n % 10));

                    t = n / 10;
                }

                if (t > 0)
                {
                    while (t > 0)
                    {
                        row.Add((byte)(t % 10));
                        t = t / 10;
                    }
                }

                space++;

                if (maxlen < row.Count)
                {
                    maxlen = row.Count;
                }

                rows.Add(row);
            }

            List<byte> output = new List<byte>();

            t = 0;

            int sum = 0;

            for (int j = 0; j < maxlen; j++)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    int value = rows[i].Count > j ? rows[i][j] : 0;
                    sum = sum + value;
                }

                output.Add((byte)(sum % 10));

                sum = sum / 10;
            }

            bool orientation = true;
            if (this.orientation != num.orientation)
            {
                orientation = false;
            }

            return new BigNumber(orientation, output);
        }

        public BigNumberDivisionResult Division(BigNumber num)
        {
            BigNumber abs = num.Absolute();

            List<byte> t = new List<byte>();

            List<byte> result = new List<byte>();


            BigNumber remainder = null;


            for (int i = this.Length() - 1; i >= 0; i--)
            {
                t.Insert(0, this.get(i));

                BigNumber a = new BigNumber(true, t);

                if (a.Compare(abs, true) == -1)
                {
                    result.Add(0);
                    continue;
                }

                BigNumber b = new BigNumber();

                byte counter = 0;
                while (a.Compare(b) > 0)
                {
                    counter++;
                    b = b.Addition(abs);
                }

                if (b.Compare(a, true) > 0)
                {
                    counter--;
                    b = b.Subtraction(abs);
                }

                remainder = a.Subtraction(b);

                t.Clear();

                for (int j = 0; j < remainder.Length(); j++)
                {
                    t.Add(remainder.get(j));
                }

                result.Add(counter);
            }

            bool orientation = true;
            if (this.orientation != num.orientation)
            {
                orientation = false;
            }

            return new BigNumberDivisionResult(
                new BigNumber(orientation, result, true),
                new BigNumber(remainder)
            );
        }

        public BigNumber Modulo(BigNumber num)
        {
            BigNumberDivisionResult result = this.Division(num);

            return result.Reminder();
        }

        public bool IsInt()
        {
            string integerMax = int.MaxValue.ToString();
            string integerMin = int.MinValue.ToString();


            if (this.IsSmaller(new BigNumber(integerMin)))
            {
                return false;
            }

            if (this.IsGreater(new BigNumber(integerMax)))
            {
                return false;
            }

            return true;
        }

        public int AsInt()
        {
            try
            {
                return int.Parse(this.ToString());
            }
            catch (FormatException)
            {

            }

            return 0;
        }



        public bool IsLong()
        {
            string integerMax = long.MaxValue.ToString();
            string integerMin = long.MinValue.ToString();


            if (this.IsSmaller(new BigNumber(integerMin)))
            {
                return false;
            }

            if (this.IsGreater(new BigNumber(integerMax)))
            {
                return false;
            }

            return true;
        }

        public long AsLong()
        {
            try
            {
                return long.Parse(this.ToString());
            }
            catch (FormatException)
            {

            }

            return 0;
        }

        public override string ToString()
        {
            string output = "";

            string outputDigitsBeforeDecimalPoint = "";

           

            if (this.digitsBeforeDecimalPoint != null && this.digitsBeforeDecimalPoint.Count > 0)
            {
                bool skipzero = true;
                for (int i = this.digitsBeforeDecimalPoint.Count - 1; i >= 0; i--)
                {
                    if (skipzero && digitsBeforeDecimalPoint[i] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        skipzero = false;
                    }

                    outputDigitsBeforeDecimalPoint = outputDigitsBeforeDecimalPoint + this.digitsBeforeDecimalPoint[i].ToString();
                }

                if (outputDigitsBeforeDecimalPoint.Length == 0)
                {
                    outputDigitsBeforeDecimalPoint = "0";
                }
                else {
                    if (this.orientation == false) {
                        outputDigitsBeforeDecimalPoint = "-" + outputDigitsBeforeDecimalPoint;
                    }
                }
            } else {
                outputDigitsBeforeDecimalPoint = "0";
            }

            string outputtDigitsAfterDecimalPoint = "";

            if (this.digitsAfterDecimalPoint != null && this.digitsAfterDecimalPoint.Count > 0)
            {
                int skipzero = 0;
                for (int i = this.digitsAfterDecimalPoint.Count - 1; i >= 0; i--)
                {
                    if (this.digitsAfterDecimalPoint[i] == 0) {
                        skipzero++;
                        continue;
                    } else {
                        if (skipzero > 0) {
                            for (int j = 0; j < skipzero; j++)
                            {
                                outputtDigitsAfterDecimalPoint = outputtDigitsAfterDecimalPoint + "0";
                            }
                            skipzero = 0;
                        }
                    }

                    outputtDigitsAfterDecimalPoint = outputtDigitsAfterDecimalPoint + this.digitsAfterDecimalPoint[i].ToString();
                }

                if (outputDigitsBeforeDecimalPoint.Length == 0)
                {
                    outputDigitsBeforeDecimalPoint = "";
                }
                else
                {
                    outputtDigitsAfterDecimalPoint = "." + outputtDigitsAfterDecimalPoint;
                }
            }
            else
            {
                outputtDigitsAfterDecimalPoint = "";
            }

            string outputExponentDigits = "";

            if (this.exponentDigits != null && this.exponentDigits.Count > 0)
            {
                bool skipzero = true;
                for (int i = this.exponentDigits.Count - 1; i >= 0; i--)
                {
                    if (skipzero && exponentDigits[i] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        skipzero = false;
                    }

                    outputExponentDigits = outputExponentDigits + exponentDigits[i].ToString();
                }

                if (outputDigitsBeforeDecimalPoint.Length == 0)
                {
                    outputDigitsBeforeDecimalPoint = "";
                }
                else
                {

                    if (this.exponentOrientation == false)
                    {
                        outputExponentDigits = "-" + outputExponentDigits;
                    }

                    outputExponentDigits = "E" + outputExponentDigits;
                }
            }
            else
            {
                outputExponentDigits = "";
            }


            return outputDigitsBeforeDecimalPoint + outputtDigitsAfterDecimalPoint + outputExponentDigits;
        }
    }
}
