namespace Rules.Framework
{
    using System;
    using System.Globalization;
    using Rules.Framework.Builder.Generic.RulesBuilder;
    using Rules.Framework.Builder.RulesBuilder;

    /// <summary>
    /// Defines extension method helpers for the rule builder API.
    /// </summary>
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Sets the new rule with the specified date begin string.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        /// <remarks>the <paramref name="dateBegin"/> is interpreted using current culture.</remarks>
        public static IRuleConfigureDateEndOptional Since(this IRuleConfigureDateBegin builder, string dateBegin)
            => builder.Since(DateTime.Parse(dateBegin, CultureInfo.CurrentCulture, DateTimeStyles.None));

        /// <summary>
        /// Sets the new rule with the specified date begin string.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        /// <remarks>the <paramref name="dateBegin"/> is interpreted using current culture.</remarks>
        public static IRuleConfigureDateEndOptional<TRuleset, TCondition> Since<TRuleset, TCondition>(this IRuleConfigureDateBegin<TRuleset, TCondition> builder, string dateBegin)
            => builder.Since(DateTime.Parse(dateBegin, CultureInfo.CurrentCulture, DateTimeStyles.None));

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, and day, assuming
        /// UTC timezone.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static IRuleConfigureDateEndOptional SinceUtc(
                    this IRuleConfigureDateBegin builder,
            int year,
            int month,
            int day)
            => builder.SinceUtc(year, month, day, 0, 0, 0);

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, day, hour, minute,
        /// and second, assuming UTC timezone.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static IRuleConfigureDateEndOptional SinceUtc(
            this IRuleConfigureDateBegin builder,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second)
            => builder.Since(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, and day, assuming
        /// UTC timezone.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static IRuleConfigureDateEndOptional<TRuleset, TCondition> SinceUtc<TRuleset, TCondition>(
            this IRuleConfigureDateBegin<TRuleset, TCondition> builder,
            int year,
            int month,
            int day)
            => builder.SinceUtc(year, month, day, 0, 0, 0);

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, day, hour, minute,
        /// and second, assuming UTC timezone.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static IRuleConfigureDateEndOptional<TRuleset, TCondition> SinceUtc<TRuleset, TCondition>(
            this IRuleConfigureDateBegin<TRuleset, TCondition> builder,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second)
            => builder.Since(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));

        /// <summary>
        /// Sets the new rule with the specified date end string.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        /// <remarks>the <paramref name="dateEnd"/> is interpreted using current culture.</remarks>
        public static IRuleBuilder Until(this IRuleConfigureDateEnd builder, string? dateEnd)
            => builder.Until(dateEnd != null ? DateTime.Parse(dateEnd, CultureInfo.CurrentCulture, DateTimeStyles.None) : null);

        /// <summary>
        /// Sets the new rule with the specified date end string.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        /// <remarks>the <paramref name="dateEnd"/> is interpreted using current culture.</remarks>
        public static IRuleBuilder<TRuleset, TCondition> Until<TRuleset, TCondition>(this IRuleConfigureDateEnd<TRuleset, TCondition> builder, string? dateEnd)
            => builder.Until(dateEnd != null ? DateTime.Parse(dateEnd, CultureInfo.CurrentCulture, DateTimeStyles.None) : null);

        /// <summary>
        /// Sets the new rule with a date end using the specified year, month, and day, assuming UTC timezone.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static IRuleBuilder UntilUtc(
                    this IRuleConfigureDateEnd builder,
            int year,
            int month,
            int day)
            => builder.Until(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, day, hour, minute,
        /// and second, assuming UTC timezone.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static IRuleBuilder UntilUtc(
            this IRuleConfigureDateEnd builder,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second)
            => builder.Until(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));

        /// <summary>
        /// Sets the new rule with a date end using the specified year, month, and day, assuming UTC timezone.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static IRuleBuilder<TRuleset, TCondition> UntilUtc<TRuleset, TCondition>(
            this IRuleConfigureDateEnd<TRuleset, TCondition> builder,
            int year,
            int month,
            int day)
            => builder.Until(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        /// <summary>
        /// Sets the new rule with a date begin using the specified year, month, day, hour, minute,
        /// and second, assuming UTC timezone.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static IRuleBuilder<TRuleset, TCondition> UntilUtc<TRuleset, TCondition>(
            this IRuleConfigureDateEnd<TRuleset, TCondition> builder,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second)
            => builder.Until(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));
    }
}