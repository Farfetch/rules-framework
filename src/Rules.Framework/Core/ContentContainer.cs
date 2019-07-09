namespace Rules.Framework.Core
{
    using System;

    /// <summary>
    /// Defines a content container with lazily loaded content.
    /// </summary>
    /// <typeparam name="TContentType"></typeparam>
    public class ContentContainer<TContentType>
    {
        private readonly Func<Type, object> getContentFunc;

        /// <summary>
        /// Creates a new <see cref="ContentContainer{TContentType}"/>.
        /// </summary>
        /// <param name="contentType">the content type.</param>
        /// <param name="getContentFunc">the function used to fetch content casted to provided type.</param>
        public ContentContainer(TContentType contentType, Func<Type, object> getContentFunc)
        {
            this.ContentType = contentType;
            this.getContentFunc = getContentFunc;
        }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public TContentType ContentType { get; }

        /// <summary>
        /// Gets the content from container casted/converted to specified <typeparamref name="TContent"/>.
        /// </summary>
        /// <typeparam name="TContent">the type to convert/cast content.</typeparam>
        /// <returns>the converted/casted content.</returns>
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