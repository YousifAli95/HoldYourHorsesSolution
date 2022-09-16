using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HoldYourHorses.Models.Entities
{
    public partial class SticksDBContext : DbContext
    {
        public SticksDBContext()
        {
        }

        public SticksDBContext(DbContextOptions<SticksDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Favourite> Favourites { get; set; } = null!;
        public virtual DbSet<Kategorier> Kategoriers { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Orderrader> Orderraders { get; set; } = null!;
        public virtual DbSet<Ordrar> Ordrars { get; set; } = null!;
        public virtual DbSet<Stick> Sticks { get; set; } = null!;
        public virtual DbSet<Tillverkningsländer> Tillverkningsländers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.Property(e => e.User).HasMaxLength(450);

                entity.HasOne(d => d.ArtikelnrNavigation)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.Artikelnr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Favourite__Artik__5DCAEF64");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Favourites__User__6E01572D");
            });

            modelBuilder.Entity<Kategorier>(entity =>
            {
                entity.ToTable("Kategorier");

                entity.HasIndex(e => e.Namn, "UQ__Kategori__737584FDBBE7B298")
                    .IsUnique();

                entity.Property(e => e.Namn).HasMaxLength(50);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Material");

                entity.HasIndex(e => e.Id, "UQ__Material__3214EC0628D961DF")
                    .IsUnique();

                entity.HasIndex(e => e.Namn, "UQ__Material__737584FD7817BA5F")
                    .IsUnique();

                entity.Property(e => e.Namn).HasMaxLength(50);
            });

            modelBuilder.Entity<Orderrader>(entity =>
            {
                entity.ToTable("Orderrader");

                entity.Property(e => e.ArtikelNamn).HasMaxLength(50);

                entity.Property(e => e.Pris).HasColumnType("money");

                entity.HasOne(d => d.ArtikelNamnNavigation)
                    .WithMany(p => p.OrderraderArtikelNamnNavigations)
                    .HasPrincipalKey(p => p.Artikelnamn)
                    .HasForeignKey(d => d.ArtikelNamn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Orderrade__Artik__47DBAE45");

                entity.HasOne(d => d.ArtikelNrNavigation)
                    .WithMany(p => p.OrderraderArtikelNrNavigations)
                    .HasForeignKey(d => d.ArtikelNr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Orderrade__Artik__46E78A0C");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Orderraders)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Orderrade__Order__45F365D3");
            });

            modelBuilder.Entity<Ordrar>(entity =>
            {
                entity.ToTable("Ordrar");

                entity.Property(e => e.Adress).HasMaxLength(50);

                entity.Property(e => e.Efternamn).HasMaxLength(50);

                entity.Property(e => e.Epost).HasMaxLength(50);

                entity.Property(e => e.Förnamn).HasMaxLength(50);

                entity.Property(e => e.Land).HasMaxLength(50);

                entity.Property(e => e.Stad).HasMaxLength(50);

                entity.Property(e => e.User).HasMaxLength(450);

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Ordrars)
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__Ordrar__User__48CFD27E");
            });

            modelBuilder.Entity<Stick>(entity =>
            {
                entity.HasKey(e => e.Artikelnr)
                    .HasName("PK__Sticks__CB7A9C835A425E7F");

                entity.HasIndex(e => e.Artikelnamn, "UQ__Sticks__6A6FEA843E50DB06")
                    .IsUnique();

                entity.Property(e => e.Artikelnamn).HasMaxLength(50);

                entity.Property(e => e.Beskrivning).HasMaxLength(1000);

                entity.Property(e => e.Pris).HasColumnType("money");

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.Sticks)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sticks__Kategori__4AB81AF0");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.Sticks)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sticks__Material__49C3F6B7");

                entity.HasOne(d => d.Tillverkningsland)
                    .WithMany(p => p.Sticks)
                    .HasForeignKey(d => d.TillverkningslandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sticks__Tillverk__4BAC3F29");
            });

            modelBuilder.Entity<Tillverkningsländer>(entity =>
            {
                entity.ToTable("Tillverkningsländer");

                entity.HasIndex(e => e.Namn, "UQ__Tillverk__737584FD3469E8DD")
                    .IsUnique();

                entity.Property(e => e.Namn).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
