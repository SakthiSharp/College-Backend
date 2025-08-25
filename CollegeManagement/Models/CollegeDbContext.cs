using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Entities
{
    public class CollegeDbContext : DbContext
    {
        public CollegeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CollegeEntity> CollegeEntities { get; set; }
        public DbSet<DepartmentEntity> DepartmentEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentEntity>()
                .HasOne(d => d.CollegeReference)
                .WithMany(c => c.DepartmentEntities)
                .HasForeignKey(d => d.CollegeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
