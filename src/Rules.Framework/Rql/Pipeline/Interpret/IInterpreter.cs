namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast.Statements;

    internal interface IInterpreter : IDisposable
    {
        Task<object> InterpretAsync(IReadOnlyList<Statement> statements);
    }
}