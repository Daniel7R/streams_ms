using Microsoft.EntityFrameworkCore;
using StreamsMS.Domain.Entities;

namespace StreamsMS.Infrastructure.Data
{
    public class StreamsDbContext: DbContext 
    {
        public DbSet<Streams> Streams { get; set; }
        public DbSet<Platforms> Platforms { get; set; }

        public StreamsDbContext(DbContextOptions<StreamsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Streams>().ToTable("streams");
            modelBuilder.Entity<Platforms>().ToTable("platforms");

            modelBuilder.Entity<Platforms>().Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Platforms>().Property(p => p.Name).HasColumnName("name");

            modelBuilder.Entity<Platforms>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Streams>().Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Streams>().Property(s => s.IdPlatform).HasColumnName("id_platform");
            modelBuilder.Entity<Streams>().Property(s => s.IdMatch).HasColumnName("id_match");
            modelBuilder.Entity<Streams>().Property(s => s.UrlStream).HasColumnName("url_stream");
            
            modelBuilder.Entity<Streams>()
                .HasIndex(s => new { s.IdMatch, s.IdPlatform })
                .IsUnique();

            modelBuilder.Entity<Streams>()
                .HasOne<Platforms>()
                .WithOne(p => p.Stream)
                .HasForeignKey<Streams>(s => s.IdPlatform);

            var listPlatforms = new List<Platforms>()
            {
                new Platforms
                {
                    Id=1,
                    Name = "YouTube"
                },
                new Platforms
                {
                    Id=2,
                    Name = "Twitch"
                },
                new Platforms
                {
                    Id=3,
                    Name="Zoom"
                },
                new Platforms
                {
                    Id=4,
                    Name = "Meet"
                }
            };

            modelBuilder.Entity<Platforms>()
                .HasData(listPlatforms);

            modelBuilder.Entity<Streams>()
                .HasData();
        }
    }
}
