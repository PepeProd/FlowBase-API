using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowBaseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowBaseAPI.DataLayer
{
    public class FlowbaseContext : DbContext
    {
        public FlowbaseContext(DbContextOptions<FlowbaseContext> options) : base(options)
        {
        }

        public DbSet<Chemical> Chemicals { get; set; }
        public DbSet<DisposedChemical> DisposedChemicals {get; set;}
        public DbSet<Location> Locations {get; set;}
        public DbSet<TempZone> TempZones {get; set;}
        public DbSet<User> Users {get; set;}
        public DbSet<MetaData> MetaData {get; set;}

        public DbSet<ChemicalFamily> ChemicalFamily {get; set;}

        public DbSet<UserRegistration> UserRegistration {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<UserRegistration>().HasIndex(u => u.Email).IsUnique();
            builder.Entity<Location>().HasIndex(u => u.Name).IsUnique();
            builder.Entity<TempZone>().HasIndex(u => u.StorageTemperature).IsUnique();
            builder.Entity<ChemicalFamily>().HasIndex(c => c.ChemicalName).IsUnique();
        } 
        
    }


}
