namespace Rules.Framework.Serialization
{
    public interface IContentSerializationProvider<in TContentType>
    {
        IContentSerializer GetContentSerializer(TContentType contentType);
    }
}