namespace Rules.Framework.Rql.Pipeline.Interpret.BuiltInFunctions
{
    using System.Linq;

    internal abstract class BuiltInFunctionBase : IRqlCallable
    {
        public int Arity => this.Parameters.Length;

        public abstract string Name { get; }

        public abstract string[] Parameters { get; }

        public abstract object Call(IInterpreter interpreter, object[] arguments);

        public override string ToString()
            => $"<built-in function> {this.ToRql()}";

        protected string ToRql()
            => $"{this.Name}({this.Parameters.Aggregate((p1, p2) => $"{p1}, {p2}")})";
    }
}