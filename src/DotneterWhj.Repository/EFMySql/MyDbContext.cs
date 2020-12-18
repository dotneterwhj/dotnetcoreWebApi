using DotneterWhj.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository
{
    public  class MyDbContext : DbContext
    {

        public DbSet<Advertisement> Advertisements { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //optionsBuilder.LogTo(log => Console.WriteLine(log));

            //optionsBuilder.UseSqlite($"Filename={AppDomain.CurrentDomain.BaseDirectory}\\webapi.db");

            //optionsBuilder.UseMySql("Database=DotnetCoreWebApi;Data Source=127.0.0.1;Port=3306;User Id=root;Password=12345678;Charset=utf8;TreatTinyAsBoolean=false;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
