namespace Rules.Framework.Rql.Messages
{
    using System;

    internal interface IMessageContainer : IDisposable
    {
        void Error(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition);

        void Warning(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition);
    }
}