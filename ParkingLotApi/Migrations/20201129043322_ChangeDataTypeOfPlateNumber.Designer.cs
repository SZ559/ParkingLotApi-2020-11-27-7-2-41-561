﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ParkingLotApi.Repository;

namespace ParkingLotApi.Migrations
{
    [DbContext(typeof(ParkingLotContext))]
    [Migration("20201129043322_ChangeDataTypeOfPlateNumber")]
    partial class ChangeDataTypeOfPlateNumber
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ParkingLotApi.Entity.OrderEntity", b =>
                {
                    b.Property<int>("OrderNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CloseTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<int?>("ParkingLotEntityId")
                        .HasColumnType("int");

                    b.Property<string>("ParkingLotName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("OrderNumber");

                    b.HasIndex("ParkingLotEntityId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ParkingLotApi.Entity.ParkingLotEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<uint>("Capacity")
                        .HasColumnType("int unsigned");

                    b.Property<string>("Location")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("ParkingLots");
                });

            modelBuilder.Entity("ParkingLotApi.Entity.OrderEntity", b =>
                {
                    b.HasOne("ParkingLotApi.Entity.ParkingLotEntity", null)
                        .WithMany("Orders")
                        .HasForeignKey("ParkingLotEntityId");
                });
#pragma warning restore 612, 618
        }
    }
}
