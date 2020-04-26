namespace Rules.Framework.Providers.MongoDb
{
    /// <summary>
    /// The settings for usage of Mongo DB as rules data source.
    /// </summary>
    public class MongoDbProviderSettings
    {
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the rules collection.
        /// </summary>
        /// <value>
        /// The name of the rules collection.
        /// </value>
        public string RulesCollectionName { get; set; }
    }
}