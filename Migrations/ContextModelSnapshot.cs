﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using db_namespace;

#nullable disable

namespace franko_bot.Migrations
{
    [DbContext(typeof(dbContext))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.6.22329.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("db_namespace.Archive", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<long?>("RelatedUserId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Archive", (string)null);
                });

            modelBuilder.Entity("db_namespace.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(12)
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Chats", (string)null);
                });

            modelBuilder.Entity("db_namespace.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(12)
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("UserSettingsCode")
                        .HasColumnType("character varying(7)");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingsCode");

                    b.ToTable("Users", (string)null);
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

                    b.ToTable("UsersData", (string)null);
                });

            modelBuilder.Entity("db_namespace.User", b =>
                {
                    b.HasOne("db_namespace.UserData", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsCode");

                    b.Navigation("UserSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
