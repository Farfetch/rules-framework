namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast.Statements;

    internal interface IInterpreter
    {
        Task<InterpretResult> InterpretAsync(IReadOnlyList<Statement> statements);
    }
}