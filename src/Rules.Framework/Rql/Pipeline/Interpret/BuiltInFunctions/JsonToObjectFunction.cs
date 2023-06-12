namespace Rules.Framework.Rql.Pipeline.Interpret.BuiltInFunctions
{
    using Newtonsoft.Json;

    internal class JsonToObjectFunction : IRqlCallable
    {
        public int Arity => 1;

        public string Name => "JSON_TO_OBJECT";

        public object Call(IInterpreter interpreter, object[] arguments)
        {
            var jsonString = (string)arguments[0];
            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        }
    }
}