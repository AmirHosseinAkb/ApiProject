using Common.Utilities;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MyApiContext:DbContext
    {
        public MyApiContext(DbContextOptions<MyApiContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterAllEntities<IEntity>();
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddGuidDefaultValueSqlConvention();
            modelBuilder.AddPluralizingTableNameConvention();
            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(entitiesAssembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
