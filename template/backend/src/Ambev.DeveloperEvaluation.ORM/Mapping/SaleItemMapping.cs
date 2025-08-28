using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Entity Framework mapping configuration for SaleItem entity
/// </summary>
public class SaleItemMapping : IEntityTypeConfiguration<SaleItem>
{
    /// <summary>
    /// Configures the SaleItem entity mapping
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        
        builder.HasKey(si => si.Id);
        
        builder.Property(si => si.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
            
        builder.Property(si => si.SaleId)
            .IsRequired();
            
        builder.Property(si => si.ProductId)
            .IsRequired();
            
        builder.Property(si => si.ProductName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(si => si.Quantity)
            .IsRequired();
            
        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(si => si.DiscountPercentage)
            .HasColumnType("decimal(5,2)");
            
        builder.Property(si => si.TotalAmount)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(si => si.IsCancelled)
            .HasDefaultValue(false);
        
        // Configure relationship with Sale
        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(si => si.SaleId);
        builder.HasIndex(si => si.ProductId);
    }
}