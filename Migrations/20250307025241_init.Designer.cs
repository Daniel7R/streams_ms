﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StreamsMS.Infrastructure.Data;

#nullable disable

namespace StreamsMS.Migrations
{
    [DbContext(typeof(StreamsDbContext))]
    [Migration("20250307025241_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StreamsMS.Domain.Entities.Platforms", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("platforms", (string)null);
                });

            modelBuilder.Entity("StreamsMS.Domain.Entities.Streams", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("IdMatch")
                        .HasColumnType("integer")
                        .HasColumnName("id_match");

                    b.Property<int>("IdPlatform")
                        .HasColumnType("integer")
                        .HasColumnName("id_platform");

                    b.Property<int>("PlatformId")
                        .HasColumnType("integer");

                    b.Property<string>("UrlStream")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("url_stream");

                    b.HasKey("Id");

                    b.HasIndex("IdPlatform")
                        .IsUnique();

                    b.HasIndex("PlatformId");

                    b.HasIndex("IdMatch", "IdPlatform")
                        .IsUnique();

                    b.ToTable("streams", (string)null);
                });

            modelBuilder.Entity("StreamsMS.Domain.Entities.Streams", b =>
                {
                    b.HasOne("StreamsMS.Domain.Entities.Platforms", null)
                        .WithOne("Stream")
                        .HasForeignKey("StreamsMS.Domain.Entities.Streams", "IdPlatform")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreamsMS.Domain.Entities.Platforms", "Platform")
                        .WithMany()
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("StreamsMS.Domain.Entities.Platforms", b =>
                {
                    b.Navigation("Stream")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
