namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Rql.Runtime.Types;

    internal class UpdateRuleArgs<TContentType>
    {
        private readonly IDictionary<UpdateRuleAttributeType, UpdateRuleAttribute> attributes;

        public UpdateRuleArgs(TContentType contentType, RqlString name)
        {
            this.ContentType = contentType;
            this.Name = name;
            this.attributes = new Dictionary<UpdateRuleAttributeType, UpdateRuleAttribute>();
        }

        public UpdateRuleAttribute[] Attributes => this.attributes.Values.ToArray();

        public TContentType ContentType { get; }

        public RqlString Name { get; }

        public void AddAttributeToUpdate(UpdateRuleAttribute attributeToUpdate)
        {
            if (attributeToUpdate is null)
            {
                throw new ArgumentNullException(nameof(attributeToUpdate));
            }

            if (this.attributes.ContainsKey(attributeToUpdate.Type))
            {
                throw new ArgumentException("The attribute to update type given already exists.", nameof(attributeToUpdate));
            }

            this.attributes[attributeToUpdate.Type] = attributeToUpdate;
        }
    }
}