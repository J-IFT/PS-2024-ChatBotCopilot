﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace copilot_chatbot.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240520141508_updatedb")]
    partial class updatedb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.11");

            modelBuilder.Entity("copilot_chatbot.Models.Export", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Exported_at")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Exports");
                });

            modelBuilder.Entity("copilot_chatbot.Models.GeneratedDataProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("GeneratedDataProducts");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Import", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Imported_at")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Imports");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Blooming_season")
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("Exposition")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Last_updated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Size")
                        .HasColumnType("TEXT");

                    b.Property<string>("Species")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("copilot_chatbot.Models.ProductKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GeneratedDataProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("KeywordId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("KeywordId");

                    b.HasIndex("GeneratedDataProductId", "KeywordId")
                        .IsUnique();

                    b.ToTable("ProductKeywords");
                });

            modelBuilder.Entity("copilot_chatbot.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ExportGeneratedDataProduct", b =>
                {
                    b.Property<int>("ExportsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GeneratedDataProductsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ExportsId", "GeneratedDataProductsId");

                    b.HasIndex("GeneratedDataProductsId");

                    b.ToTable("ExportGeneratedDataProduct");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Export", b =>
                {
                    b.HasOne("copilot_chatbot.Models.User", "User")
                        .WithMany("Exports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("copilot_chatbot.Models.GeneratedDataProduct", b =>
                {
                    b.HasOne("copilot_chatbot.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Import", b =>
                {
                    b.HasOne("copilot_chatbot.Models.Product", "Product")
                        .WithMany("Imports")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("copilot_chatbot.Models.User", "User")
                        .WithMany("Imports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("copilot_chatbot.Models.ProductKeyword", b =>
                {
                    b.HasOne("copilot_chatbot.Models.GeneratedDataProduct", "GeneratedDataProduct")
                        .WithMany("ProductKeyword")
                        .HasForeignKey("GeneratedDataProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("copilot_chatbot.Models.Keyword", "Keyword")
                        .WithMany("ProductKeywords")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GeneratedDataProduct");

                    b.Navigation("Keyword");
                });

            modelBuilder.Entity("ExportGeneratedDataProduct", b =>
                {
                    b.HasOne("copilot_chatbot.Models.Export", null)
                        .WithMany()
                        .HasForeignKey("ExportsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("copilot_chatbot.Models.GeneratedDataProduct", null)
                        .WithMany()
                        .HasForeignKey("GeneratedDataProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("copilot_chatbot.Models.GeneratedDataProduct", b =>
                {
                    b.Navigation("ProductKeyword");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Keyword", b =>
                {
                    b.Navigation("ProductKeywords");
                });

            modelBuilder.Entity("copilot_chatbot.Models.Product", b =>
                {
                    b.Navigation("Imports");
                });

            modelBuilder.Entity("copilot_chatbot.Models.User", b =>
                {
                    b.Navigation("Exports");

                    b.Navigation("Imports");
                });
#pragma warning restore 612, 618
        }
    }
}
