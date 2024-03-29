namespace Rules.Framework.Source
{
    internal sealed class GetRulesFilteredArgs<TContentType>
    {
        public TContentType ContentType { get; set; }

        public string Name { get; set; }

        public int? Priority { get; set; }
    }
}