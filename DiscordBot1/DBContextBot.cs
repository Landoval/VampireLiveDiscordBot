using System;
using System.Collections.Generic;
using System.Data.Common;
using DiscordBot1.Database;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace DiscordBot1
{
     public class DBContextBot : DbContext
    {
        public DBContextBot() : base()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //var connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnecionString"].ConnectionString;
            //optionsBuilder.UseSqlServer(connectionString);

            optionsBuilder.UseSqlServer(@"Server=.\SQLSERVER2017;Database=DiscordBot;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharakterWert>()
            .HasOne(e => e.character)
            .WithMany(b => b.charakterwertListe).HasForeignKey(e => e.characterID);


            modelBuilder.Entity<User>()
            .HasOne(e => e.Charakter)
            .WithOne(b => b.User).HasForeignKey<Charakterblatt>(e => e.UserID);
        }

        public DbSet<Charakterblatt> SetCharakterblatt { get; set; }
        public DbSet<CharakterWert> SetCharakterwert { get; set; }
        public DbSet<User> SetUser { get; set; }


        
    }
}
