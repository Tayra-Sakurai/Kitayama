// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Matsugasaki.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Matsugasaki.Contexts
{
    public class MatsugasakiContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<LargeCategory> LargeCategories { get; set; }
        public DbSet<MediumCategory> MediumCategories { get; set; }
        public DbSet<SmallCategory> SmallCategories { get; set; }
        public DbSet<Method> Methods { get; set; }

        private string DbPath { get; }

        public MatsugasakiContext()
        {
            DbPath = Path.Combine(ApplicationData.Current.SharedLocalFolder.Path, "Matsugasaki.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                $"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(
                t =>
                {
                    t.HasKey( e => e.Id );

                    t.HasOne(e => e.ExpenseMethod)
                    .WithMany(e => e.ExpenseItems)
                    .HasForeignKey(e => e.ExpenseMethodId)
                    .IsRequired(false);

                    t.HasOne(e => e.IncomeMethod)
                    .WithMany(e => e.IncomeItems)
                    .HasForeignKey(e => e.IncomeMethodId)
                    .IsRequired(false);

                    t.Property(e => e.DateTime)
                    .HasConversion(
                        v => v.ToString("O"),
                        v => DateTime.Parse(v)
                    )
                    .HasDefaultValueSql("datetime()");

                    t.Property(e => e.Name)
                    .IsRequired();

                    t.Property(e => e.Description)
                    .IsRequired();
                });

            modelBuilder.Entity<Method>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<LargeCategory>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<MediumCategory>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<SmallCategory>()
                .Property(e => e.Name)
                .IsRequired();
        }
    }
}
