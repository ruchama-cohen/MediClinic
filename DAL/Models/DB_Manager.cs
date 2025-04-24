using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DB_Manager : DbContext
{
    public DB_Manager()
    {
    }

    public DB_Manager(DbContextOptions<DB_Manager> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentsSlot> AppointmentsSlots { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<BranchToServiceProvider> BranchToServiceProviders { get; set; }

    public virtual DbSet<ClinicService> ClinicServices { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }

    public virtual DbSet<WorkHour> WorkHours { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\למודים\\c# project\\MediClinic\\DAL\\data\\DB.mdf;Integrated Security=True;Connect Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2A1B2979DC2E");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__tmp_ms_x__8ECDFCA2E7D038B4");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__6383C8BA");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__SlotI__6477ECF3");
        });

        modelBuilder.Entity<AppointmentsSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Appointm__0A124A4FE839B173");

            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Branch).WithMany(p => p.AppointmentsSlots)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Branc__60A75C0F");

            entity.HasOne(d => d.Provider).WithMany(p => p.AppointmentsSlots)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Provi__5FB337D6");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branches__A1682FA505E4713B");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.BranchManagerId).HasColumnName("BranchManagerID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.Address).WithMany(p => p.Branches)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Branches__Addres__3A81B327");

            entity.HasOne(d => d.BranchManager).WithMany(p => p.Branches)
                .HasForeignKey(d => d.BranchManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Branches__Branch__3B75D760");
        });

        modelBuilder.Entity<BranchToServiceProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BranchTo__3214EC0762546FCD");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchToServiceProviders)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BranchToS__Branc__70DDC3D8");

            entity.HasOne(d => d.ServicProvider).WithMany(p => p.BranchToServiceProviders)
                .HasForeignKey(d => d.ServicProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BranchToS__Servi__6FE99F9F");
        });

        modelBuilder.Entity<ClinicService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__ClinicSe__C51BB0EA79CDD871");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC3465DE6F86C");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.PatientName)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.PatientPassword)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.Address).WithMany(p => p.Patients)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patients__Addres__4316F928");
        });

        modelBuilder.Entity<ServiceProvider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK__ServiceP__B54C689D9AE2A9A5");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.BranchToProviderId).HasColumnName("BranchToProviderID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.BranchToProvider).WithMany(p => p.ServiceProviders)
                .HasForeignKey(d => d.BranchToProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicePr__Branc__71D1E811");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceProviders)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicePr__Servi__44FF419A");
        });

        modelBuilder.Entity<WorkHour>(entity =>
        {
            entity.HasKey(e => e.WorkHourId).HasName("PK__WorkHour__75C23DCEF54159A7");

            entity.Property(e => e.WorkHourId).HasColumnName("WorkHourID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Branch).WithMany(p => p.WorkHours)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkHours__Branc__5165187F");

            entity.HasOne(d => d.Provider).WithMany(p => p.WorkHours)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkHours__Provi__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
