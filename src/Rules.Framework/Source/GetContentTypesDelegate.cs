namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal delegate Task<IEnumerable<string>> GetContentTypesDelegate(GetContentTypesArgs args);
}