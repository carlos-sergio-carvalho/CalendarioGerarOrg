using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CalendarioGerarOrg.Models;

namespace CalendarioGerarOrg.Models
{
    public class AppDbContext : DbContext
    {

        //public DbSet<subsede> Subsede { get; set; }
        public DbSet<recesso> Recesso { get; set; }
        public DbSet<feriado> Feriado { get; set; }
        public DbSet<cidade> Cidade { get; set; }
        public DbSet<subsede> Subsede { get; set; }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseSqlite($"Filename={App._dbPath}");
            optionsBuilder
                .UseLazyLoadingProxies()
                //.UseMySql("Server=six-sql2014;Database=calendariogerar;Uid=root;Pwd=six.2009;");
                //.UseMySql("Server=mysql.www2.sixtech.com.br;Database=www2;User Id=www2;Password=six2009;");
                .UseMySql("Server=node66382-gerarcalendario.jelastic.saveincloud.net;Database=calendario;User Id=root;Password=3UGHxW6Wur;",
                new MySqlServerVersion(new Version(5, 7, 32))
                , mySqlOptions => mySqlOptions
                             .CharSetBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSetBehavior.NeverAppend)
                );
        }

        
    }
}
