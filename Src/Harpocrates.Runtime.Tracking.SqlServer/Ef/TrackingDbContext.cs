using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Harpocrates.Runtime.Tracking.SqlServer.Ef
{
    public class TrackingDbContext : DbContext
    {
        public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options) { }

        public DbSet<Entities.Transaction> Transactions { get; set; }
        public DbSet<Entities.Attempt> Attempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ForSq

            modelBuilder.Entity<Entities.Transaction>().ToTable("Transactions", "tracking");
            //    .HasOne(i => i.ParentTransaction)
            //    .WithOne()
            //    .HasForeignKey<Entities.Transaction>(i => i.TrackingId);
            //.HasIndex(i => i.TransactionId);

            modelBuilder.Entity<Entities.Attempt>().ToTable("Attempts", "tracking");
        }
    }
}
