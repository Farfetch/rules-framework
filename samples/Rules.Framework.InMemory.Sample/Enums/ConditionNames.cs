namespace Rules.Framework.InMemory.Sample.Enums
{
    internal enum ConditionNames
    {
        /// <summary>
        /// no condition defined
        /// </summary>
        None = 0,

        /// <summary>
        /// condition to filter by royal numbers
        /// </summary>
        RoyalNumber = 1,

        /// <summary>
        /// condition to filter if the number can be divided by 3
        /// </summary>
        CanNumberBeDividedBy3 = 2,

        /// <summary>
        /// condition to filter if the number is prime
        /// </summary>
        IsPrimeNumber = 3,

        /// <summary>
        /// condition to filter number sums
        /// </summary>
        SumAll = 4
    }
}