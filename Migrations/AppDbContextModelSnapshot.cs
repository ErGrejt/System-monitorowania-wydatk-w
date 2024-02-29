﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Models;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WebApplication1.Models.Jedzenie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Cena")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Waluta")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Jedzenie");
                });

            modelBuilder.Entity("WebApplication1.Models.Zdrowie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Cena")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Waluta")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Zdrowie");
                });
#pragma warning restore 612, 618
        }
    }
}
