﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EloBot.Migrations
{
    [DbContext(typeof(EloBotContext))]
    partial class EloBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<ulong>("FirstMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("FirstMessageId")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("FirstPlayerConfirmedWin")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("FirstPlayerDiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("PendingWinnerId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("SecondMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("SecondMessageId")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("SecondPlayerConfirmedWin")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("SecondPlayerDiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MatchId");

                    b.HasIndex("FirstPlayerDiscordId");

                    b.HasIndex("SecondPlayerDiscordId");

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

            modelBuilder.Entity("Match", b =>
                {
                    b.HasOne("Player", "FirstPlayer")
                        .WithMany()
                        .HasForeignKey("FirstPlayerDiscordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Player", "SecondPlayer")
                        .WithMany()
                        .HasForeignKey("SecondPlayerDiscordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FirstPlayer");

                    b.Navigation("SecondPlayer");
                });
#pragma warning restore 612, 618
        }
    }
}
