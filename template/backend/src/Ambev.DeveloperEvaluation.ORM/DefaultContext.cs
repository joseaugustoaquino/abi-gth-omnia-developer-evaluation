using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

/// <summary>
/// The default database context for the application
/// </summary>
public class DefaultContext : DbContext
{
    /// <summary>
    /// Gets or sets the Users DbSet
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Gets or sets the Sales DbSet
    /// </summary>
    public DbSet<Sale> Sales { get; set; }
    
    /// <summary>
    /// Gets or sets the SaleItems DbSet
    /// </summary>
    public DbSet<SaleItem> SaleItems { get; set; }

    /// <summary>
    /// Initializes a new instance of DefaultContext
    /// </summary>
    /// <param name="options">The database context options</param>
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply existing configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        // Apply new Sales configurations
        modelBuilder.ApplyConfiguration(new SaleMapping());
        modelBuilder.ApplyConfiguration(new SaleItemMapping());
    }
}
public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(
               connectionString,
               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.WebApi")
        );

        return new DefaultContext(builder.Options);
    }
}