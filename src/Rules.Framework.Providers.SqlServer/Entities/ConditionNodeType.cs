// <auto-generated>
// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rules.Framework.SqlServer.Models
{
    // ConditionNodeTypes
    public class ConditionNodeType
    {
        public int Code { get; set; } // Code (Primary key)
        public string Name { get; set; } // Name (length: 100)

        // Reverse navigation

        /// <summary>
        /// Child ConditionNodes where [ConditionNodes].[ConditionNodeTypeCode] point to this entity (FK_ConditionNodes_ConditionNodeTypes)
        /// </summary>
        public virtual ICollection<ConditionNode> ConditionNodes { get; set; } // ConditionNodes.FK_ConditionNodes_ConditionNodeTypes

        public ConditionNodeType()
        {
            ConditionNodes = new List<ConditionNode>();
        }
    }

}
// </auto-generated>
