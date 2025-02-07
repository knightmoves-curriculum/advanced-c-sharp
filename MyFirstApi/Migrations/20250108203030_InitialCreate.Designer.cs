﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyFirstApi.Models;

#nullable disable

namespace MyFirstApi.Migrations
{
    [DbContext(typeof(WeatherForecastDbContext))]
    [Migration("20250108203030_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("WeatherAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AlertMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WeatherForecastId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WeatherForecastId")
                        .IsUnique();

                    b.ToTable("WeatherAlerts");
                });

            modelBuilder.Entity("WeatherComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommentMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WeatherForecastId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WeatherForecastId");

                    b.ToTable("WeatherComments");
                });

            modelBuilder.Entity("WeatherForecast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<int>("TemperatureC")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TemperatureF")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("WeatherForecasts");
                });

            modelBuilder.Entity("WeatherAlert", b =>
                {
                    b.HasOne("WeatherForecast", "WeatherForecast")
                        .WithOne("Alert")
                        .HasForeignKey("WeatherAlert", "WeatherForecastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WeatherForecast");
                });

            modelBuilder.Entity("WeatherComment", b =>
                {
                    b.HasOne("WeatherForecast", "WeatherForecast")
                        .WithMany("Comments")
                        .HasForeignKey("WeatherForecastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WeatherForecast");
                });

            modelBuilder.Entity("WeatherForecast", b =>
                {
                    b.Navigation("Alert");

                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
