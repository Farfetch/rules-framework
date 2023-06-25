namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Types;

    internal class JsonToObjectFunction : BuiltInFunctionBase
    {
        public override string Name => "JSON_TO_OBJECT";

        public override Parameter[] Parameters => new[] { new Parameter(RqlTypes.String, "jsonString") };

        public override RqlType ReturnType => RqlTypes.Object;

        public override object Call(IInterpreter interpreter, object instance, object[] arguments)
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
            var json = JsonConvert.DeserializeObject<object>(jsonString.Value);
            return ConvertToRqlObject((IDictionary<string, JToken>)json);
        }

        private static RqlObject ConvertToRqlObject(IDictionary<string, JToken> values)
        {
            var rqlObject = new RqlObject();
            foreach (var kv in values)
            {
                rqlObject[kv.Key] = new RqlAny(kv.Value.Type switch
                {
                    JTokenType.Null => new RqlNothing(),
                    JTokenType.Integer => new RqlInteger(kv.Value.Value<int>()),
                    JTokenType.Float => new RqlDecimal(kv.Value.Value<decimal>()),
                    JTokenType.String => new RqlString(kv.Value.Value<string>()),
                    JTokenType.Boolean => new RqlBool(kv.Value.Value<bool>()),
                    JTokenType.Date => new RqlDate(kv.Value.Value<DateTime>()),
                    JTokenType.Object => ConvertToRqlObject((IDictionary<string, JToken>)kv.Value),
                    _ => throw new NotSupportedException($"The JSON token type '{kv.Value.Type}' is not supported."),
                });
            }

            return rqlObject;
        }
    }
}