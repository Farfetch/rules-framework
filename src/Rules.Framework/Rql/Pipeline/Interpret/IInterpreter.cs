namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Statements;

    internal interface IInterpreter
    {
        Task<object> InterpretAsync(IReadOnlyList<Statement> statements);
    }
}