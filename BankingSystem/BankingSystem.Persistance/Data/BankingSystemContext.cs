using System;
using System.Collections.Generic;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Persistance.Data;

public partial class BankingSystemContext : DbContext
{
    public BankingSystemContext()
    {
    }

    public BankingSystemContext(DbContextOptions<BankingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }


    public virtual DbSet<Customer> Customers { get; set; }


    public virtual DbSet<Employee> Employees { get; set; }


    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }
    
    public virtual DbSet<Tenant> Tenants { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=bankingsystemdb;Username=postgres;Password=mysequel1!");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accounts_pkey");

            entity.ToTable("accounts");

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

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("accounts_customerid_fkey");
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.ToTable("customers", "branch1");

            entity.HasIndex(e => e.Name, "customers_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('employee_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Branchid).HasColumnName("branchid");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");  // Maps to the "events" table

                entity.HasKey(e => e.EventId);  // Primary key

                entity.Property(e => e.EventId)
                    .HasColumnName("event_id")
                    .HasDefaultValueSql("uuid_generate_v4()");  // PostgreSQL UUID generation

                entity.Property(e => e.AggregateId)
                    .IsRequired()
                    .HasColumnName("aggregate_id");

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("event_type");

                entity.Property(e => e.EventData)
                    .IsRequired()
                    .HasColumnName("event_data");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");  // PostgreSQL function for current timestamp
            });
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions", "branch1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
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

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Accountid)
                .HasConstraintName("transactions_accountid_fkey");
        });
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId).HasName("tenant_pkey");

            entity.ToTable("tenants", "public"); // Specify the public schema

            entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(e => e.EncryptedTenantKey).HasColumnName("encrypted_tenant_key").IsRequired();
            entity.Property(e => e.TenantName)
                .HasMaxLength(255)
                .HasColumnName("tenant_name")
                .IsRequired();
            entity.Property(e => e.ConnectionString)
                .HasMaxLength(255)
                .HasColumnName("connection_string")
                .IsRequired();
        });
        

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
