using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CampingEF.Models;

public partial class MarconnesDbContext : DbContext
{
    public MarconnesDbContext()
    {
    }

    public MarconnesDbContext(DbContextOptions<MarconnesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CampingPlace> CampingPlaces { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=marconnes-groep6.database.windows.net;Initial Catalog=marconnes-db;Persist Security Info=True;User ID=pieter;Password=Nog-sterker-wachtwoord-1;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampingPlace>(entity =>
        {
            entity.HasKey(e => e.PlaceId);

            entity.Property(e => e.PlaceId).HasColumnName("PlaceID");
            entity.Property(e => e.GroundType).HasMaxLength(50);
            entity.Property(e => e.PlaceNumber).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
