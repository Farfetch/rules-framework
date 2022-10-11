namespace Rules.Framework.Admin.Dashboard.Sample.Helper
{
    using System;
    using System.Linq;

    internal static class NumberHelper
    {
        public static bool CanNumberBeDividedBy3(this int number)
        {
            return (number % 3 == 0);
        }

        public static bool IsPrime(this int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        public static string SumAll(this int number)
        {
            return number.ToString().ToCharArray().Select(part => Convert.ToInt32(part)).Sum().ToString();
        }
    }
}