using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryUpdateApi.Models
{
    public class RetailTransSalesContext:DbContext
    {
        public RetailTransSalesContext(DbContextOptions<RetailTransSalesContext> options)
            :base(options)
        {

        }

        public DbSet<RETAILTRANSACTIONSALESTRANS> RetailTransSalesItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RETAILTRANSACTIONSALESTRANS>().ToTable("RETAILTRANSACTIONSALESTRANS");
            modelBuilder.Entity<RETAILTRANSACTIONSALESTRANS>().HasNoKey();

            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(RetailTransSalesItem).Assembly);
        }

    }
}
