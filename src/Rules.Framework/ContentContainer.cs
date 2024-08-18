namespace Rules.Framework
{
    using System;

    /// <summary>
    /// Defines a content container with lazily loaded content.
    /// </summary>
    public class ContentContainer
    {
        private readonly Func<Type, object> getContentFunc;

        /// <summary>
        /// Creates a new <see cref="ContentContainer"/>.
        /// </summary>
        /// <param name="getContentFunc">
        /// the function used to fetch content casted to provided type.
        /// </param>
        public ContentContainer(Func<Type, object> getContentFunc)
        {
            this.getContentFunc = getContentFunc;
        }

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