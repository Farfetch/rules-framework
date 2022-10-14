// <auto-generated>
// ReSharper disable All


namespace Rules.Framework.SqlServer.Models
{
   using Microsoft.Data.SqlClient;
   using Microsoft.EntityFrameworkCore;
   using System;
   using System.Data.SqlTypes;

    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************

    public class RulesFrameworkDbContext : DbContext, IRulesFrameworkDbContext
    {
        public RulesFrameworkDbContext()
        {
        }

        public RulesFrameworkDbContext(DbContextOptions<RulesFrameworkDbContext> options)
            : base(options)
        {
        }

        public DbSet<ConditionNode> ConditionNodes { get; set; } // ConditionNodes
        public DbSet<ConditionNodeRelation> ConditionNodeRelations { get; set; } // ConditionNodeRelations
        public DbSet<ConditionNodeType> ConditionNodeTypes { get; set; } // ConditionNodeTypes
        public DbSet<ConditionType> ConditionTypes { get; set; } // ConditionTypes
        public DbSet<ContentType> ContentTypes { get; set; } // ContentTypes
        public DbSet<DataType> DataTypes { get; set; } // DataTypes
        public DbSet<LogicalOperator> LogicalOperators { get; set; } // LogicalOperators
        public DbSet<Operator> Operators { get; set; } // Operators
        public DbSet<Rule> Rules { get; set; } // Rules

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //@"Data Source=sqlserver.docker.internal;User ID=sa; Password=Finance123.; Initial Catalog=rules-framework-sample; MultipleActiveResultSets=True"
                //@"Data Source=localhost;Initial Catalog=rules-framework-sample;Integrated Security=True;MultipleActiveResultSets=True"
                optionsBuilder.UseSqlServer(@"Data Source=localhost;Initial Catalog=rules-framework-sample;Integrated Security=True;MultipleActiveResultSets=True")
                    .UseLazyLoadingProxies();
            }
        }

        public bool IsSqlParameterNull(SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == DBNull.Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ConditionNodeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionNodeRelationConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionNodeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DataTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LogicalOperatorConfiguration());
            modelBuilder.ApplyConfiguration(new OperatorConfiguration());
            modelBuilder.ApplyConfiguration(new RuleConfiguration());
        }

    }
}
// </auto-generated>
