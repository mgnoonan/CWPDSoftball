using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softball.Mvc4.Utilities
{
    public static class StringExtensions
    {
        public static string Left(this string value, int length)
        {
            if (length >= value.Length)
                return value;

            return value.Substring(0, length);
        }

        public static string Right(this string value, int length)
        {
            if (length >= value.Length)
                return value;

            return value.Substring(value.Length - length, length);
        }

    }
}
