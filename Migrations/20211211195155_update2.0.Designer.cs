﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using YoghurtBank.Infrastructure;

#nullable disable

namespace YoghurtBank.Migrations
{
    [DbContext(typeof(YoghurtContext))]
    [Migration("20211211195155_update2.0")]
    partial class update20
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("YoghurtBank.Data.Model.CollaborationRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("IdeaId")
                        .HasColumnType("integer");

                    b.Property<int>("RequesteeId")
                        .HasColumnType("integer");

                    b.Property<int>("RequesterId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IdeaId");

                    b.HasIndex("RequesteeId");

                    b.HasIndex("RequesterId");

                    b.ToTable("CollaborationRequests");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Idea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AmountOfCollaborators")
                        .HasColumnType("integer");

                    b.Property<int>("CreatorId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("Open")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Posted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<TimeSpan>("TimeToComplete")
                        .HasColumnType("interval");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Ideas");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Student", b =>
                {
                    b.HasBaseType("YoghurtBank.Data.Model.User");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Supervisor", b =>
                {
                    b.HasBaseType("YoghurtBank.Data.Model.User");

                    b.HasDiscriminator().HasValue("Supervisor");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.CollaborationRequest", b =>
                {
                    b.HasOne("YoghurtBank.Data.Model.Idea", "Idea")
                        .WithMany()
                        .HasForeignKey("IdeaId");

                    b.HasOne("YoghurtBank.Data.Model.Supervisor", "Requestee")
                        .WithMany("CollaborationRequests")
                        .HasForeignKey("RequesteeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YoghurtBank.Data.Model.Student", "Requester")
                        .WithMany("CollaborationRequests")
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Idea");

                    b.Navigation("Requestee");

                    b.Navigation("Requester");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Idea", b =>
                {
                    b.HasOne("YoghurtBank.Data.Model.Supervisor", "Creator")
                        .WithMany("Ideas")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Student", b =>
                {
                    b.Navigation("CollaborationRequests");
                });

            modelBuilder.Entity("YoghurtBank.Data.Model.Supervisor", b =>
                {
                    b.Navigation("CollaborationRequests");

                    b.Navigation("Ideas");
                });
#pragma warning restore 612, 618
        }
    }
}
