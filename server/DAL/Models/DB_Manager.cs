﻿using System;
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

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ClinicService> ClinicServices { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }

    public virtual DbSet<Street> Streets { get; set; }

    public virtual DbSet<WorkHour> WorkHours { get; set; }

    // עדכן את DB_Manager.cs עם לוגים מפורטים
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            Console.WriteLine("=== DATABASE CONNECTION DEBUG ===");

            // מציאת הנתיב הבסיסי של הפרויקט
            string currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"1. Current Directory: {currentDirectory}");

            // חזור אחורה עד שנמצא את תיקיית הפרויקט הראשית (MediClinic)
            string projectRoot = currentDirectory;
            int steps = 0;

            while (!Directory.Exists(Path.Combine(projectRoot, "DAL")) &&
                   Directory.GetParent(projectRoot) != null && steps < 10)
            {
                projectRoot = Directory.GetParent(projectRoot).FullName;
                steps++;
                Console.WriteLine($"2.{steps} Checking parent: {projectRoot}");
                Console.WriteLine($"   - DAL folder exists: {Directory.Exists(Path.Combine(projectRoot, "DAL"))}");
            }

            Console.WriteLine($"3. Final Project Root: {projectRoot}");
            Console.WriteLine($"4. DAL folder found: {Directory.Exists(Path.Combine(projectRoot, "DAL"))}");

            // בדוק את תוכן התיקיות
            string dalFolder = Path.Combine(projectRoot, "DAL");
            if (Directory.Exists(dalFolder))
            {
                Console.WriteLine($"5. DAL folder contents:");
                foreach (var item in Directory.GetFileSystemEntries(dalFolder))
                {
                    Console.WriteLine($"   - {Path.GetFileName(item)}");
                }

                string dataFolder = Path.Combine(dalFolder, "data");
                Console.WriteLine($"6. Data folder exists: {Directory.Exists(dataFolder)}");

                if (Directory.Exists(dataFolder))
                {
                    Console.WriteLine($"7. Data folder contents:");
                    foreach (var file in Directory.GetFiles(dataFolder))
                    {
                        var fileInfo = new FileInfo(file);
                        Console.WriteLine($"   - {Path.GetFileName(file)} ({fileInfo.Length} bytes)");
                    }
                }
            }

            // נתיב למסד הנתונים
            string dbPath = Path.Combine(projectRoot, "DAL", "data", "DB.mdf");
            Console.WriteLine($"8. Database path: {dbPath}");
            Console.WriteLine($"9. Database file exists: {File.Exists(dbPath)}");

            if (File.Exists(dbPath))
            {
                var fileInfo = new FileInfo(dbPath);
                Console.WriteLine($"10. Database file size: {fileInfo.Length} bytes");
                Console.WriteLine($"11. Database file last modified: {fileInfo.LastWriteTime}");
            }

            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;
                         AttachDbFilename={dbPath};
                         Integrated Security=True;
                         Connect Timeout=30";

            Console.WriteLine($"12. Connection String: {connectionString}");
            Console.WriteLine("=== END DEBUG ===");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__tmp_ms_x__091C2A1B8E00964C");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.PostalCode).HasMaxLength(10);

            entity.HasOne(d => d.City).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Addresses__CityI__787EE5A0");

            entity.HasOne(d => d.Street).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.StreetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Addresses__Stree__797309D9");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__tmp_ms_x__8ECDFCA2E7D038B4");

            entity.HasIndex(e => e.SlotId, "UQ_Appointments_Slot").IsUnique();

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");

            entity.HasOne(d => d.PatientKeyNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__7E37BEF6");

            entity.HasOne(d => d.Slot).WithOne(p => p.Appointment)
                .HasForeignKey<Appointment>(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__SlotI__6477ECF3");
        });

        modelBuilder.Entity<AppointmentsSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Appointm__0A124A4FE839B173");

            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");

            entity.HasOne(d => d.Branch).WithMany(p => p.AppointmentsSlots)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Branc__60A75C0F");

            entity.HasOne(d => d.ProviderKeyNavigation).WithMany(p => p.AppointmentsSlots)
                .HasForeignKey(d => d.ProviderKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Provi__02FC7413");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branches__A1682FA505E4713B");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.BranchManagerId).HasColumnName("BranchManagerID");
            entity.Property(e => e.BranchName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.Address).WithMany(p => p.Branches)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Branches__Addres__74AE54BC");

            entity.HasOne(d => d.BranchManager).WithMany(p => p.Branches)
                .HasForeignKey(d => d.BranchManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Branches__Branch__3B75D760");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B76DCF01A5F");

            entity.ToTable("City");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ClinicService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__ClinicSe__C51BB0EA79CDD871");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientKey).HasName("PK__tmp_ms_x__E92C0061174DA5CF");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PatientId)
                .HasMaxLength(100)
                .HasColumnName("PatientID");
            entity.Property(e => e.PatientName).HasMaxLength(100);
            entity.Property(e => e.PatientPassword).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.Address).WithMany(p => p.Patients)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patients__Addres__7F2BE32F");
        });

        modelBuilder.Entity<ServiceProvider>(entity =>
        {
            entity.HasKey(e => e.ProviderKey).HasName("PK__tmp_ms_x__8DE43C5EFF223585");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.ProviderId)
                .HasMaxLength(100)
                .HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Branch).WithMany(p => p.ServiceProviders)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicePr__Branc__00200768");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceProviders)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicePr__Servi__01142BA1");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.HasKey(e => e.StreetId).HasName("PK__Street__6270EB3A6FD6D3A6");

            entity.ToTable("Street");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.City).WithMany(p => p.Streets)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Street__CityId__71D1E811");
        });

        modelBuilder.Entity<WorkHour>(entity =>
        {
            entity.HasKey(e => e.WorkHourId).HasName("PK__WorkHour__75C23DCEF54159A7");

            entity.Property(e => e.WorkHourId).HasColumnName("WorkHourID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");

            entity.HasOne(d => d.Branch).WithMany(p => p.WorkHours)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkHours__Branc__5165187F");

            entity.HasOne(d => d.ProviderKeyNavigation).WithMany(p => p.WorkHours)
                .HasForeignKey(d => d.ProviderKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkHours__Provi__02084FDA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
