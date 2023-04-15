﻿// <auto-generated />
using System;
using FaceRecognitionWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FaceRecognitionWebAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.AugmentedFace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("FaceToTrainId")
                        .HasColumnType("int");

                    b.Property<string>("ImageFile")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FaceToTrainId");

                    b.ToTable("AugmentedFaces");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceExpression", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ImageFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FaceExpressions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ImageFile = "face_1.jpg",
                            Name = "Resting Face"
                        },
                        new
                        {
                            Id = 2,
                            ImageFile = "face_2.jpg",
                            Name = "Resting Face with Eyes Closed"
                        },
                        new
                        {
                            Id = 3,
                            ImageFile = "face_3.jpg",
                            Name = "Open Mouth Face"
                        },
                        new
                        {
                            Id = 4,
                            ImageFile = "face_4.jpg",
                            Name = "Open Mouth Face with Eyes Closed"
                        },
                        new
                        {
                            Id = 5,
                            ImageFile = "face_5.jpg",
                            Name = "Smiling Face"
                        },
                        new
                        {
                            Id = 6,
                            ImageFile = "face_6.jpg",
                            Name = "Smiling Face with Eyes Closed"
                        },
                        new
                        {
                            Id = 7,
                            ImageFile = "face_7.jpg",
                            Name = "Duck Face"
                        },
                        new
                        {
                            Id = 8,
                            ImageFile = "face_8.jpg",
                            Name = "Duck Face with Eyes Closed"
                        });
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceRecognitionStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("FaceToRecognizeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsRecognized")
                        .HasColumnType("bit");

                    b.Property<int>("PredictedPersonId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FaceToRecognizeId")
                        .IsUnique();

                    b.HasIndex("PredictedPersonId");

                    b.ToTable("FaceRecognitionStatuses");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceToRecognize", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ImageFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LoggedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("FacesToRecognize");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceToTrain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("FaceExpressionId")
                        .HasColumnType("int");

                    b.Property<string>("ImageFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FaceExpressionId");

                    b.HasIndex("PersonId");

                    b.ToTable("FacesToTrain");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PairId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Persons");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Admin",
                            LastName = "Admin",
                            PairId = 1
                        });
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.AugmentedFace", b =>
                {
                    b.HasOne("FaceRecognitionWebAPI.Models.FaceToTrain", "FaceToTrain")
                        .WithMany("AugmentedFaces")
                        .HasForeignKey("FaceToTrainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FaceToTrain");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceRecognitionStatus", b =>
                {
                    b.HasOne("FaceRecognitionWebAPI.Models.FaceToRecognize", "FaceToRecognize")
                        .WithOne("FaceRecognitionStatus")
                        .HasForeignKey("FaceRecognitionWebAPI.Models.FaceRecognitionStatus", "FaceToRecognizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FaceRecognitionWebAPI.Models.Person", "PredictedPerson")
                        .WithMany("FaceRecognitionStatuses")
                        .HasForeignKey("PredictedPersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FaceToRecognize");

                    b.Navigation("PredictedPerson");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceToTrain", b =>
                {
                    b.HasOne("FaceRecognitionWebAPI.Models.FaceExpression", "FaceExpression")
                        .WithMany("FacesToTrain")
                        .HasForeignKey("FaceExpressionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FaceRecognitionWebAPI.Models.Person", "Person")
                        .WithMany("FacesToTrain")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FaceExpression");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceExpression", b =>
                {
                    b.Navigation("FacesToTrain");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceToRecognize", b =>
                {
                    b.Navigation("FaceRecognitionStatus");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.FaceToTrain", b =>
                {
                    b.Navigation("AugmentedFaces");
                });

            modelBuilder.Entity("FaceRecognitionWebAPI.Models.Person", b =>
                {
                    b.Navigation("FaceRecognitionStatuses");

                    b.Navigation("FacesToTrain");
                });
#pragma warning restore 612, 618
        }
    }
}
