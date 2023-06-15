namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using Newtonsoft.Json;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Types;

    internal class JsonToObjectFunction : BuiltInFunctionBase
    {
        public override string Name => "JSON_TO_OBJECT";

        public override Parameter[] Parameters => new[] { new Parameter(RqlTypes.String, "jsonString") };

        public override RqlType ReturnType => RqlTypes.Object;

        public override object Call(IInterpreter interpreter, object[] arguments)
        {
            if (arguments[0] is not RqlString)
            {
                throw new RuntimeException(
                    $"Error on {this.Name}: expected string argument.",
                    this.ToRql(),
                    RqlSourcePosition.Empty,
                    RqlSourcePosition.Empty);
            }

            var jsonString = (RqlString)arguments[0];
            return new RqlObject((object)JsonConvert.DeserializeObject<dynamic>(jsonString.Value));
        }
    }
}