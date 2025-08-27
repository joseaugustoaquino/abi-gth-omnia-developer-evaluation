using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Entity Framework mapping configuration for Sale entity
/// </summary>
public class SaleMapping : IEntityTypeConfiguration<Sale>
{
    /// <summary>
    /// Configures the Sale entity mapping
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
            
        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.SaleDate)
            .IsRequired();
            
        builder.Property(s => s.CustomerId)
            .IsRequired();
            
        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(s => s.BranchId)
            .IsRequired();
            
        builder.Property(s => s.BranchName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(s => s.TotalAmount)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        builder.Property(s => s.UpdatedAt);
        
        // Configure relationship with SaleItems
        builder.HasMany(s => s.Items)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();
            
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.SaleDate);
    }
}