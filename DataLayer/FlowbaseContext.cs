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
        
    }
}
