namespace Rules.Framework.Providers.MongoDb
{
    /// <summary>
    /// The settings for usage of Mongo DB as rules data source.
    /// </summary>
    public class MongoDbProviderSettings
    {
        private const string DefaultContentTypesCollectionName = "content_types";

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbProviderSettings"/> class.
        /// </summary>
        public MongoDbProviderSettings()
        {
            this.ContentTypesCollectionName = DefaultContentTypesCollectionName;
        }

        /// <summary>
        /// Gets or sets the name of the content types collection.
        /// </summary>
        /// <value>The name of the content types collection.</value>
        public string ContentTypesCollectionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the rules collection.
        /// </summary>
        /// <value>The name of the rules collection.</value>
        public string RulesCollectionName { get; set; }
    }
}