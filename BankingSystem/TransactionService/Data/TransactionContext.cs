
using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data
{
    public partial class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options)
            : base(options)
        {
        }

        // DbSet for the Transaction model
        public DbSet<Transaction> Transactions { get; set; }

        // Optionally include Account if it's relevant for the microservice
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("transactions_pkey");

                entity.ToTable("transactions", "branch1");  // Assuming the schema is "branch1"

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountid");

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("amount");

                entity.Property(e => e.Date)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("transactions_accountid_fkey");
            });

            // Configure the Account entity if needed
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("accounts_pkey");

                entity.ToTable("accounts", "branch1");

                entity.HasIndex(e => e.Accountnumber, "accounts_accountnumber_key").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Accountnumber)
                    .HasMaxLength(50)
                    .HasColumnName("accountnumber");
                entity.Property(e => e.Balance)
                    .HasPrecision(18, 2)
                    .HasColumnName("balance");
                entity.Property(e => e.Branchid).HasColumnName("branchid");
                entity.Property(e => e.Customerid).HasColumnName("customerid");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.Customerid)
                    .HasConstraintName("accounts_customerid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
