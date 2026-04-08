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

    public virtual DbSet<Gebruiker> Gebruikers { get; set; }

    public virtual DbSet<Reserveringen> Reserveringens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampingPlace>(entity =>
        {
            entity.HasKey(e => e.PlaceNumber);

            entity.Property(e => e.PlaceNumber).ValueGeneratedNever();
            entity.Property(e => e.GroundType).HasMaxLength(50);
            entity.Property(e => e.MaxGuests).ValueGeneratedOnAdd();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Gebruiker>(entity =>
        {
            entity.Property(e => e.GebruikerId).HasColumnName("Gebruiker_id");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Naam).HasMaxLength(50);
            entity.Property(e => e.Telefoonnummer).HasMaxLength(50);

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
            entity.Property(e => e.Volwassenen).HasColumnName("volwassenen");
            entity.Property(e => e.TotaalPrijs)
          .HasColumnName("TotaalPrijs")
          .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.AccomodatieNavigation).WithMany(p => p.Reserveringens)
                .HasForeignKey(d => d.Accomodatie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserveringen_Reserveringen");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Reserveringens)
                .HasForeignKey(d => d.GebruikerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserveringen_Gebruikers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
