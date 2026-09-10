using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepairRequest.Web.Models;
using RepairRequestEntity = global::RepairRequest.Web.Models.RepairRequest;

namespace RepairRequest.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<RepairRequestEntity> RepairRequests => Set<RepairRequestEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var repairRequest = builder.Entity<RepairRequestEntity>();
        repairRequest.HasKey(request => request.Id);
        repairRequest.Property(request => request.RequestNumber).HasMaxLength(45).IsRequired();
        repairRequest.HasIndex(request => request.RequestNumber).IsUnique();
        repairRequest.Property(request => request.RequesterId).HasMaxLength(450).IsRequired();
        repairRequest.Property(request => request.EquipmentCode).HasMaxLength(50).IsRequired();
        repairRequest.Property(request => request.Location).HasMaxLength(100).IsRequired();
        repairRequest.Property(request => request.Title).HasMaxLength(150).IsRequired();
        repairRequest.Property(request => request.Description).HasMaxLength(RepairRequestEntity.MaximumDescriptionLength).IsRequired();
        repairRequest.HasIndex(request => new { request.RequesterId, request.CreatedUtc });
    }
}
