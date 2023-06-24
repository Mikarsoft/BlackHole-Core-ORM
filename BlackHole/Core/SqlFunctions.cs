
using BlackHole.Entities;

namespace BlackHole.Core
{
    public static class SqlFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="property"></param>
        /// <param name="otherTypesProperty"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool SqlEqualTo<TOther>(this string property ,Func<TOther,string> otherTypesProperty, int Id ) where TOther: BlackHoleEntity<int>
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="property"></param>
        /// <param name="otherTypesProperty"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool SqlEqualTo<TOther>(this int property, Func<TOther, int> otherTypesProperty, int Id) where TOther : BlackHoleEntity<int>
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="property"></param>
        /// <param name="otherTypesProperty"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="afterDate"></param>
        /// <returns></returns>
        public static bool SqlDateAfter(this DateTime value, DateTime afterDate)
        {
            return value > afterDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="beforeDate"></param>
        /// <returns></returns>
        public static bool SqlDateBefore(this DateTime value , DateTime beforeDate)
        {
            return value < beforeDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this DateTime value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this int value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this short value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this decimal value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this double value)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMin(this long value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlMax(this DateTime value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public static int SqlPlus(this int value, int otherValue)
        {
            return value + otherValue;
        }

        public static int SqlPlus(this short value, short otherValue)
        {
            return value + otherValue;
        }

        public static decimal SqlPlus(this decimal value, decimal otherValue)
        {
            return value + otherValue;
        }

        public static double SqlPlus(this double value, double otherValue)
        {
            return value + otherValue;
        }

        public static long SqlPlus(this long value, long otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public static int SqlMinus(this int value, int otherValue)
        {
            return value - otherValue;
        }

        public static int SqlMinus(this short value, short otherValue)
        {
            return value - otherValue;
        }

        public static decimal SqlMinus(this decimal value, decimal otherValue)
        {
            return value - otherValue;
        }

        public static double SqlMinus(this double value, double otherValue)
        {
            return value - otherValue;
        }

        public static long SqlMinus(this long value, long otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        public static bool SqlLike(this string value, string similarValue)
        {
            return value.Contains(similarValue);
        }

        public static int SqlLength(this string value)
        {
            return value.Length;
        }

        public static string SqlReplace(this string value, string replaceValue, string withValue)
        {
            return value.Replace(replaceValue, withValue);
        }
    }
}
