using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pluralize.NET;

namespace Common.Utilities;

public static class ModelBuilderExtensions
{
    public static void RegisterAllEntities<BaseEntity>(this ModelBuilder modelBuilder,params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && typeof(BaseEntity).IsAssignableFrom(t));
        foreach (var type in types)
            modelBuilder.Entity(type);
    }

    public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
    {
        IEnumerable<IMutableForeignKey> fks = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var fk in fks)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, Type propertyType,
        string propertyName, string defaultValue)
    {
        var properties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == propertyType && p.Name.Equals(propertyName,StringComparison.OrdinalIgnoreCase));
        if(properties.Any())
            foreach (var property in properties) 
                property.SetDefaultValueSql(defaultValue);
    }

    public static void AddGuidDefaultValueSqlConvention(this ModelBuilder modelBuilder)
    {
        AddDefaultValueSqlConvention(modelBuilder,typeof(Guid),"id","NEWSEQUENTIALID()");
    }

    public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        var pluralizer = new Pluralizer();
        foreach (var model in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = model.GetTableName();
            model.SetTableName(pluralizer.Pluralize(tableName));
        }
    }

    public static void AddsingularizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        var pluralizer = new Pluralizer();
        foreach (var model in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = model.GetTableName();
            model.SetTableName(pluralizer.Singularize(tableName));
        }
    }
}