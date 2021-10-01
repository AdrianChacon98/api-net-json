using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webApi2.Models;

namespace webApi2.Context
{
    public class AppDbContext : DbContext
    {   

       public DbSet<User> Users {set;get;}
       //public DbSet<Role> Roles {set;get;}

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /*
        private const string connectionString = "server=localhost;port=3306;database=webApi2;user=root;password=$chancho2012$";
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,new MySqlServerVersion(new Version(8, 0, 11)));


        }*/


        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);
            
            /*
            builder.Entity<Role>(entity=>
            {
                entity.ToTable("role");
                //entity.HasIndex(c=>c.id);
                
            });
            
            */

            builder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                /*entity.Property(e=>e.id)*/
                entity.HasKey(e=>e.id);
                entity.Property(e=>e.id).ValueGeneratedOnAdd();
                entity.Property(p=>p.dateCreated)
                    .HasColumnType("datetime2")
                    .HasPrecision(0);


                //entity.HasOne(e=>e.role).WithOne().HasForeignKey<Role>(c=>c.id);//.HasForeignKey("role").HasConstraintName("FK_Role_User");
            });
        }
        
    }
}