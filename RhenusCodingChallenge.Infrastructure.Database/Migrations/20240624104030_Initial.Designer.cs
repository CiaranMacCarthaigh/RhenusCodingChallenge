﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RhenusCodingChallenge.Infrastructure.Database.Context;

#nullable disable

namespace RhenusCodingChallenge.Infrastructure.Database.Migrations
{
    [DbContext(typeof(EventStorageDbContext))]
    [Migration("20240624104030_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RhenusCodingChallenge.Infrastructure.Database.DomainEventStorageObject", b =>
                {
                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("AggregateId", "Version");

                    b.ToTable("DomainEventStorageDbSet");
                });
#pragma warning restore 612, 618
        }
    }
}
