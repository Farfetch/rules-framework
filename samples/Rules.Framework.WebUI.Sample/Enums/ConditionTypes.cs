namespace Rules.Framework.WebUI.Sample.Enums
{
    public enum ConditionTypes
    {
        /// <summary>
        /// no condition type defined
        /// </summary>
        None = 0,

        /// <summary>
        /// condition type to filter by royal numbers
        /// </summary>
        RoyalNumber = 1,

        /// <summary>
        /// condition type to filter if the number can be divided by 3
        /// </summary>
        CanNumberBeDividedBy3 = 2,

        /// <summary>
        /// condition type to filter if the number is prime
        /// </summary>
        IsPrimeNumber = 3,

        /// <summary>
        /// condition type to filter number sums
        /// </summary>
        SumAll = 4
    }
}