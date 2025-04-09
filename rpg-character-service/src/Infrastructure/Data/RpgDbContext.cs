using Microsoft.EntityFrameworkCore;
using RPGCharacterService.Infrastructure.Data.Entities;

namespace RPGCharacterService.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core database context for the RPG character application.
    /// </summary>
    public class RpgDbContext : DbContext
    {
        public RpgDbContext(DbContextOptions<RpgDbContext> options) : base(options)
        {
        }

        public DbSet<CharacterEntity> Characters { get; set; }
        public DbSet<AbilityScoreEntity> AbilityScores { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ArmorStatsEntity> ArmorStats { get; set; }
        public DbSet<WeaponStatsEntity> WeaponStats { get; set; }
        public DbSet<CharacterEquipmentEntity> CharacterEquipment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Character entity
            modelBuilder.Entity<CharacterEntity>(entity =>
            {
                entity.ToTable("Characters");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Race).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subrace).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Class).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HitPoints).IsRequired().HasDefaultValue(10);
                entity.Property(e => e.Level).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.InitFlags).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure AbilityScore entity
            modelBuilder.Entity<AbilityScoreEntity>(entity =>
            {
                entity.ToTable("AbilityScores");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.AbilityType).IsRequired();
                entity.Property(e => e.Score).IsRequired();
                entity.HasIndex(e => new { e.CharacterId, e.AbilityType }).IsUnique();

                // Configure relationship to Character
                entity.HasOne(e => e.Character)
                    .WithMany(c => c.AbilityScores)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Currency entity
            modelBuilder.Entity<CurrencyEntity>(entity =>
            {
                entity.ToTable("Currencies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Copper).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Silver).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Electrum).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Gold).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Platinum).IsRequired().HasDefaultValue(0);
                entity.HasIndex(e => e.CharacterId).IsUnique();

                // Configure relationship to Character
                entity.HasOne(e => e.Character)
                    .WithOne(c => c.Currency)
                    .HasForeignKey<CurrencyEntity>(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Item entity
            modelBuilder.Entity<ItemEntity>(entity =>
            {
                entity.ToTable("Items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EquipmentType);
            });

            // Configure ArmorStats entity
            modelBuilder.Entity<ArmorStatsEntity>(entity =>
            {
                entity.ToTable("ArmorStats");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.BaseArmorClass).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.ArmorType).IsRequired();
                entity.HasIndex(e => e.ItemId).IsUnique();

                // Configure relationship to Item
                entity.HasOne(e => e.Item)
                    .WithOne(i => i.ArmorStats)
                    .HasForeignKey<ArmorStatsEntity>(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure WeaponStats entity
            modelBuilder.Entity<WeaponStatsEntity>(entity =>
            {
                entity.ToTable("WeaponStats");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.WeaponProperties).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.RangeType).IsRequired().HasDefaultValue(0);
                entity.HasIndex(e => e.ItemId).IsUnique();

                // Configure relationship to Item
                entity.HasOne(e => e.Item)
                    .WithOne(i => i.WeaponStats)
                    .HasForeignKey<WeaponStatsEntity>(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CharacterEquipment entity
            modelBuilder.Entity<CharacterEquipmentEntity>(entity =>
            {
                entity.ToTable("CharacterEquipment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasIndex(e => e.CharacterId).IsUnique();

                // Configure relationships
                entity.HasOne(e => e.Character)
                    .WithOne(c => c.Equipment)
                    .HasForeignKey<CharacterEquipmentEntity>(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MainHandItem)
                    .WithMany()
                    .HasForeignKey(e => e.MainHandItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.OffHandItem)
                    .WithMany()
                    .HasForeignKey(e => e.OffHandItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ArmorItem)
                    .WithMany()
                    .HasForeignKey(e => e.ArmorItemId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
