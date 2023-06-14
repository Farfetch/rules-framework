namespace Rules.Framework.Rql.Pipeline.Interpret.BuiltInFunctions
{
    using Newtonsoft.Json;

    internal class JsonToObjectFunction : BuiltInFunctionBase
    {
        public override string Name => "JSON_TO_OBJECT";

        public override string[] Parameters => new[] { "jsonString" };

        public override object Call(IInterpreter interpreter, object[] arguments)
        {
            if (arguments[0] is not string)
            {
                throw new RuntimeException(
                    $"Error on {this.Name}: expected string argument.",
                    this.ToRql(),
                    RqlSourcePosition.Empty,
                    RqlSourcePosition.Empty);
            }

            var jsonString = (string)arguments[0];
            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        }
    }
}