using entities;
using Microsoft.EntityFrameworkCore;
using mxcd.dbContextExtended;
using System;
using System.Linq;

namespace data
{
    public partial class EnneagramContext : DbContextExtended
    {
        public EnneagramContext(DbContextOptions<EnneagramContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Enneatype> Enneatypes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<Triad> Triads { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Enneatype>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.KeyName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Triad>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.KeyName)
                .IsRequired()
                .HasMaxLength(50);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.KeyName)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Value).IsRequired();

                entity.Property(e => e.KeyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Value)
                    .IsRequired();
            });

            CreateDataBase();
        }

        private void CreateDataBase()
        {
            this.Database.EnsureCreated();
            var versionField = Configurations.Find(enums.Configurations.VERSION);
            var version = int.Parse(versionField?.Value ?? "0");

            switch (version)
            {
                case 0:
                    Configurations.Add(new Configuration { Id = new Guid(), KeyName = enums.Configurations.VERSION, Value = "1" });

                    Triads.Add(new Triad { Id = 1, KeyName = "TRIAD_INSTINCT" });
                    Triads.Add(new Triad { Id = 2, KeyName = "TRIAD_HEART" });
                    Triads.Add(new Triad { Id = 3, KeyName = "TRIAD_HEAD" });


                    short i = 1;
                    (new Array[9]).ToList().ForEach(x =>
                    {
                        Enneatypes.Add(new Enneatype
                        {
                            Id = i,
                            KeyName =$"ENNEATYPE_{i++}"
                        });
                    });


            }
        }
    }
}
