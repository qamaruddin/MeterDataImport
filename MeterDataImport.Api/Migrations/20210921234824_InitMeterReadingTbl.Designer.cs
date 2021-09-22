﻿// <auto-generated />
using System;
using MeterDataImport.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MeterDataImport.Api.Migrations
{
    [DbContext(typeof(MeterDataImportApDbContext))]
    [Migration("20210921234824_InitMeterReadingTbl")]
    partial class InitMeterReadingTbl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MeterDataImport.Api.Domain.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("MeterDataImport.Api.Domain.MeterReading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReadingDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("MeterReadings");
                });

            modelBuilder.Entity("MeterDataImport.Api.Domain.MeterReading", b =>
                {
                    b.HasOne("MeterDataImport.Api.Domain.Account", "Account")
                        .WithMany("MeterReadings")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("MeterDataImport.Api.Domain.Account", b =>
                {
                    b.Navigation("MeterReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
