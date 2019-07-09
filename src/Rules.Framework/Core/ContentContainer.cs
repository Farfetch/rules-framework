namespace Rules.Framework.Core
{
    using System;

    public class ContentContainer<TContentType>
    {
        private readonly Func<Type, object> getContentFunc;

        public ContentContainer(TContentType contentType, Func<Type, object> getContentFunc)
        {
            this.ContentType = contentType;
            this.getContentFunc = getContentFunc;
        }

        public TContentType ContentType { get; }

        public TContent GetContentAs<TContent>()
        {
            try
            {
                return (TContent)this.getContentFunc.Invoke(typeof(TContent));
            }
            catch (InvalidCastException ice)
            {
                throw new ContentTypeException($"Cannot cast content to provided type as {nameof(TContent)}: {typeof(TContent).FullName}", ice);
            }
        }
    }
}