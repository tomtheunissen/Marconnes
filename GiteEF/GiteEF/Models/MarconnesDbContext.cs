using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GiteEF.Models;

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

    public virtual DbSet<Gebruiker> Gebruikers { get; set; }

    public virtual DbSet<Gite> Gites { get; set; }

    public virtual DbSet<HotelRoom> HotelRooms { get; set; }

    public virtual DbSet<Reserveringen> Reserveringens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=marconnes-db;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampingPlace>(entity =>
        {
            entity.HasKey(e => e.PlaceNumber);

            entity.Property(e => e.GroundType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Gebruiker>(entity =>
        {
            entity.Property(e => e.GebruikerId).HasColumnName("Gebruiker_id");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Naam).HasMaxLength(50);
            entity.Property(e => e.Telefoonnummer).HasMaxLength(50);
        });

        modelBuilder.Entity<Gite>(entity =>
        {
            entity.HasKey(e => e.GiteNumber);

            entity.Property(e => e.Price).HasColumnType("decimal(10, 0)");
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.HasKey(e => e.RoomNumber);

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Reserveringen>(entity =>
        {
            entity.HasKey(e => e.ReserveringId);

            entity.ToTable("Reserveringen");

            entity.Property(e => e.ReserveringId).HasColumnName("Reservering_id");
            entity.Property(e => e.Begindatum).HasColumnName("begindatum");
            entity.Property(e => e.Einddatum).HasColumnName("einddatum");
            entity.Property(e => e.GebruikerId).HasColumnName("gebruiker_id");
            entity.Property(e => e.Kinderen07).HasColumnName("kinderen07");
            entity.Property(e => e.Kinderen712).HasColumnName("kinderen712");
            entity.Property(e => e.TotaalPrijs).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Volwassenen).HasColumnName("volwassenen");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Reserveringens)
                .HasForeignKey(d => d.GebruikerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserveringen_Gebruikers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
