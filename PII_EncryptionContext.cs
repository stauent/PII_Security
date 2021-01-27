using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PII_Security
{
    public partial class PII_EncryptionContext : DbContext
    {
        public PII_EncryptionContext()
        {
        }

        public PII_EncryptionContext(DbContextOptions<PII_EncryptionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source = OPTIMUS; Initial Catalog = PII_Encryption; Persist Security Info = True; User ID = ma; Password=I8well4sure;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PiiAnnualSalary)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("PII_AnnualSalary");

                entity.Property(e => e.PiiDateOfBirth)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("PII_DateOfBirth");

                entity.Property(e => e.PiiSocialInsuranceNumber)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("PII_SocialInsuranceNumber");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
