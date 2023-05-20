
using BlackHole.Entities;

namespace BlackHole.Core
{
    public static class SqlFunctions
    {
        public static bool SqlEqualTo<TOther>(this string property ,Func<TOther,string> otherTypesProperty, int Id ) where TOther: BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this int property, Func<TOther, int> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this short property, Func<TOther, short> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this long property, Func<TOther, long> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this double property, Func<TOther, double> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this float property, Func<TOther, float> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this byte[] property, Func<TOther, byte[]> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this string property, Func<TOther, string> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this int property, Func<TOther, int> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this short property, Func<TOther, short> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this long property, Func<TOther, long> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this double property, Func<TOther, double> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this float property, Func<TOther, float> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this byte[] property, Func<TOther, byte[]> otherTypesProperty, Guid Id) where TOther : BlackHoleEntity<Guid>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this string property, Func<TOther, string> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this int property, Func<TOther, int> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this short property, Func<TOther, short> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this long property, Func<TOther, long> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this double property, Func<TOther, double> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this float property, Func<TOther, float> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlEqualTo<TOther>(this byte[] property, Func<TOther, byte[]> otherTypesProperty, string Id) where TOther : BlackHoleEntity<string>
        {
            return true;
        }

        public static bool SqlDateAfter(this DateTime value, DateTime afterDate)
        {
            return value > afterDate;
        }

        public static bool SqlDateBefore(this DateTime value , DateTime beforeDate)
        {
            return value < beforeDate;
        }

        public static bool SqlMin(this DateTime value)
        {
            return true;
        }

        public static bool SqlMin(this int value)
        {
            return true;
        }

        public static bool SqlMin(this short value)
        {
            return true;
        }

        public static bool SqlMin(this decimal value)
        {
            return true;
        }

        public static bool SqlMin(this double value)
        {
            return true;
        }

        public static bool SqlMin(this long value)
        {
            return true;
        }

        public static bool SqlMax(this DateTime value)
        {
            return true;
        }

        public static bool SqlMax(this int value)
        {
            return true;
        }

        public static bool SqlMax(this short value)
        {
            return true;
        }

        public static bool SqlMax(this decimal value)
        {
            return true;
        }

        public static bool SqlMax(this double value)
        {
            return true;
        }

        public static bool SqlMax(this long value)
        {
            return true;
        }

        public static string SqlConcat(this string value, string secondValue)
        {
            return value + secondValue;
        }

        public static int SqlSum(this int value, int otherValue)
        {
            return value + otherValue;
        }

        public static int SqlSum(this short value, short otherValue)
        {
            return value + otherValue;
        }

        public static decimal SqlSum(this decimal value, decimal otherValue)
        {
            return value + otherValue;
        }

        public static double SqlSum(this double value, double otherValue)
        {
            return value + otherValue;
        }

        public static long SqlSum(this long value, long otherValue)
        {
            return value + otherValue;
        }

        public static int SqlAverage(this int value)
        {
            return value;
        }

        public static short SqlAverage(this short value)
        {
            return value;
        }

        public static decimal SqlAverage(this decimal value)
        {
            return value;
        }

        public static double SqlAverage(this double value)
        {
            return value;
        }

        public static long SqlAverage(this long value)
        {
            return value;
        }

        public static double SqlPower(this int value, int power)
        {
            return Math.Pow(value,power);
        }

        public static double SqlPower(this short value, int power)
        {
            return Math.Pow(value, power);
        }

        public static double SqlPower(this long value, int power)
        {
            return Math.Pow(value, power);
        }

        public static double SqlPower(this double value, int power)
        {
            return Math.Pow(value, power);
        }

        public static int SqlAbsolut(this int value)
        {
            return Math.Abs(value);
        }

        public static short SqlAbsolut(this short value)
        {
            return Math.Abs(value);
        }

        public static long SqlAbsolut(this long value)
        {
            return Math.Abs(value);
        }

        public static double SqlAbsolut(this double value)
        {
            return Math.Abs(value);
        }

        public static decimal SqlAbsolut(this decimal value)
        {
            return Math.Abs(value);
        }

        public static decimal SqlRound(this decimal value)
        {
            return Math.Round(value);
        }

        public static double SqlRound(this double value)
        {
            return Math.Round(value);
        }

        public static decimal SqlRound(this decimal value, int digits)
        {
            return Math.Round(value,digits);
        }

        public static double SqlRound(this double value, int digits)
        {
            return Math.Round(value,digits);
        }

        public static decimal SqlFloor(this decimal value)
        {
            return Math.Floor(value);
        }

        public static double SqlFloor(this double value)
        {
            return Math.Floor(value);
        }

        public static decimal SqlCeiling(this decimal value)
        {
            return Math.Ceiling(value);
        }

        public static double SqlCeiling(this double value)
        {
            return Math.Ceiling(value);
        }

        public static bool SqlLike(this string value, string similarValue)
        {
            return value.Contains(similarValue);
        }

        public static string SqlUpper(this string value)
        {
            return value.ToUpper();
        }

        public static string SqlLower(this string value)
        {
            return value.ToLower();
        }

        public static int SqlLength(this string value)
        {
            return value.Length;
        }

        public static string SqlReplace(this string value, string replaceValue, string withValue)
        {
            return value.Replace(replaceValue, withValue);
        }

        public static string SqlRight(this string value, int lettersFromRight)
        {
            if(value.Length > lettersFromRight)
            {
                return value.Substring(value.Length - lettersFromRight, value.Length - 1);
            }

            return value;
        }

        public static string SqlLeft(this string value, int lettersFromLeft)
        {
            if (value.Length > lettersFromLeft)
            {
                return value.Substring(0, lettersFromLeft - 1);
            }

            return value;
        }

        public static string SqlReverse(this string value)
        {
            string result = "";

            for(int i = 0; i < value.Length; i++)
            {
                result += value[value.Length - i -1];
            }

            return result;
        }
    }
}
