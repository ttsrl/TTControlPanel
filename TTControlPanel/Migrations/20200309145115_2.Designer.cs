﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TTControlPanel.Services;

namespace TTControlPanel.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20200309145115_2")]
    partial class _2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TTControlPanel.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CAP");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Province");

                    b.Property<string>("Street");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("TTControlPanel.Models.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ClientId");

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("TTControlPanel.Models.ApplicationVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ApplicationId");

                    b.Property<string>("Notes");

                    b.Property<DateTime>("ReleaseDate");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("ApplicationsVersions");
                });

            modelBuilder.Entity("TTControlPanel.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId");

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("VAT");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("TTControlPanel.Models.HID", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Hids");
                });

            modelBuilder.Entity("TTControlPanel.Models.License", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ActivateDateTime");

                    b.Property<bool>("Active");

                    b.Property<int?>("ApplicationVersionId");

                    b.Property<bool>("Banned");

                    b.Property<int?>("ClientId");

                    b.Property<string>("ConfirmCode");

                    b.Property<int?>("HidId");

                    b.Property<string>("Notes");

                    b.Property<int?>("ProductKeyId");

                    b.Property<DateTime>("ReleaseDate");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationVersionId");

                    b.HasIndex("ClientId");

                    b.HasIndex("HidId");

                    b.HasIndex("ProductKeyId");

                    b.ToTable("Licenses");
                });

            modelBuilder.Entity("TTControlPanel.Models.ProductKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("GenerateDateTime");

                    b.Property<int?>("GenerateUserId");

                    b.Property<string>("Key");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("GenerateUserId");

                    b.ToTable("ProductKeys");
                });

            modelBuilder.Entity("TTControlPanel.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("GrantLogin");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("TTControlPanel.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Barcode");

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int?>("RoleId");

                    b.Property<string>("Username");

                    b.Property<bool?>("Visible")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TTControlPanel.Models.Application", b =>
                {
                    b.HasOne("TTControlPanel.Models.Client")
                        .WithMany("Applications")
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TTControlPanel.Models.ApplicationVersion", b =>
                {
                    b.HasOne("TTControlPanel.Models.Application", "Application")
                        .WithMany("ApplicationVersions")
                        .HasForeignKey("ApplicationId");
                });

            modelBuilder.Entity("TTControlPanel.Models.Client", b =>
                {
                    b.HasOne("TTControlPanel.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");
                });

            modelBuilder.Entity("TTControlPanel.Models.License", b =>
                {
                    b.HasOne("TTControlPanel.Models.ApplicationVersion", "ApplicationVersion")
                        .WithMany("Licences")
                        .HasForeignKey("ApplicationVersionId");

                    b.HasOne("TTControlPanel.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("TTControlPanel.Models.HID", "Hid")
                        .WithMany()
                        .HasForeignKey("HidId");

                    b.HasOne("TTControlPanel.Models.ProductKey", "ProductKey")
                        .WithMany()
                        .HasForeignKey("ProductKeyId");
                });

            modelBuilder.Entity("TTControlPanel.Models.ProductKey", b =>
                {
                    b.HasOne("TTControlPanel.Models.User", "GenerateUser")
                        .WithMany()
                        .HasForeignKey("GenerateUserId");
                });

            modelBuilder.Entity("TTControlPanel.Models.User", b =>
                {
                    b.HasOne("TTControlPanel.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });
#pragma warning restore 612, 618
        }
    }
}
