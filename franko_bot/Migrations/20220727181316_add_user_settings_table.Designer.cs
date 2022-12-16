﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using db_namespace;

#nullable disable

namespace franko_bot.Migrations
{
    [DbContext(typeof(dbContext))]
    [Migration("20220727181316_add_user_settings_table")]
    partial class add_user_settings_table
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.6.22329.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("db_namespace.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(12)
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("UserSettingsCode")
                        .IsRequired()
                        .HasColumnType("character varying(7)");

                    b.Property<int>("UserSettingsId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingsCode");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("db_namespace.UserData", b =>
                {
                    b.Property<string>("Code")
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<int>("Course")
                        .HasColumnType("integer");

                    b.Property<string>("Faculty")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Group")
                        .HasColumnType("integer");

                    b.Property<string>("Speciality")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Code");

                    b.ToTable("UsersData");
                });

            modelBuilder.Entity("db_namespace.User", b =>
                {
                    b.HasOne("db_namespace.UserData", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
