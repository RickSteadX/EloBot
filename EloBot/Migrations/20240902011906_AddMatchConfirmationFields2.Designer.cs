﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EloBot.Migrations
{
    [DbContext(typeof(EloBotContext))]
    [Migration("20240902011906_AddMatchConfirmationFields2")]
    partial class AddMatchConfirmationFields2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EloChange")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("PendingWinnerId")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Player1ConfirmedWin")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Player1Id")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Player2ConfirmedWin")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Player2Id")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MatchId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Player", b =>
                {
                    b.Property<ulong>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Elo")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Rank")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("DiscordId");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
