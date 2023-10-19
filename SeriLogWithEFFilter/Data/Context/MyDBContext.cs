using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SeriLogWithEFFilter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogWithEFFilter.Data.Context
{
    public class MyDBContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); });
        public string DbPath { get; }

        public MyDBContext() 
        {
            var path = @"SeriLogWithEFFilter\Data\";
            DbPath = System.IO.Path.Join(path, "MyData.db");
            Database.EnsureCreated();
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors  { get; set; }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}")
                   .UseLoggerFactory(MyLoggerFactory);
            SQLitePCL.Batteries.Init();
        }
            


    }
}
