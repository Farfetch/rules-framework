namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System.Linq;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;

    internal abstract class BuiltInFunctionBase : ICallable
    {
        public int Arity => this.Parameters.Length;

        public abstract string Name { get; }

        public abstract Parameter[] Parameters { get; }

        public abstract RqlType ReturnType { get; }

        public abstract IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments);

        public override string ToString()
            => $"[built-in function] {this.ToRql()}";

        protected string ToRql()
            => $"<{this.ReturnType.Name}> {this.Name}({this.Parameters.Select(p => p.ToString()).Aggregate((p1, p2) => $"{p1}, {p2}")})";
    }
}