using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PII_Security
{
    public partial class PII_EncryptionContext : DbContext
    {
        protected string PII_EncryptionConnectionString { get; set; }
        public PII_EncryptionContext(string PII_EncryptionConnectionString)
        {
            this.PII_EncryptionConnectionString = PII_EncryptionConnectionString;
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
                optionsBuilder.UseSqlServer(PII_EncryptionConnectionString);
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
