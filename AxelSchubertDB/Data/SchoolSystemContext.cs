using System;
using System.Collections.Generic;
using AxelSchubertDB.Models;
using Microsoft.EntityFrameworkCore;

namespace AxelSchubertDB.Data;

public partial class SchoolSystemContext : DbContext
{
    public SchoolSystemContext()
    {
    }

    public SchoolSystemContext(DbContextOptions<SchoolSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-9QARTN3;Initial Catalog=SchoolSystem;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187F1618DD4");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(30);
            entity.Property(e => e.TeacherIdFk).HasColumnName("TeacherID_FK");

            entity.HasOne(d => d.TeacherIdFkNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherIdFk)
                .HasConstraintName("FK__Courses__Teacher__3B75D760");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF101DD653E");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ContactInfo).HasMaxLength(30);
            entity.Property(e => e.FullName).HasMaxLength(40);
            entity.Property(e => e.Occupation).HasMaxLength(20);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A7942EE68EA");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ContactInfo).HasMaxLength(30);
            entity.Property(e => e.FullName).HasMaxLength(40);
            entity.Property(e => e.SocialSecurityNumber).HasMaxLength(13);
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => e.StudentCourseId).HasName("PK__StudentC__7E3E2FB21906176F");

            entity.ToTable("StudentCourse");

            entity.Property(e => e.StudentCourseId).HasColumnName("StudentCourseID");
            entity.Property(e => e.CourseIdFk).HasColumnName("CourseID_FK");
            entity.Property(e => e.Grade).HasMaxLength(5);
            entity.Property(e => e.StudentIdFk).HasColumnName("StudentID_FK");

            entity.HasOne(d => d.CourseIdFkNavigation).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.CourseIdFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentCo__Cours__3F466844");

            entity.HasOne(d => d.StudentIdFkNavigation).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.StudentIdFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentCo__Stude__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
