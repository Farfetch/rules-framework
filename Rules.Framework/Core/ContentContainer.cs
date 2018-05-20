using System;

namespace Rules.Framework.Core
{
    public class ContentContainer<TContentType>
    {
        private readonly Func<Type, object> getContentFunc;

        public ContentContainer(TContentType contentType, Func<Type, object> getContentFunc)
        {
            this.ContentType = contentType;
            this.getContentFunc = getContentFunc;
        }

        public TContentType ContentType { get; }

        public TContent GetContentAs<TContent>() => (TContent)this.getContentFunc.Invoke(typeof(TContent));
    }
}