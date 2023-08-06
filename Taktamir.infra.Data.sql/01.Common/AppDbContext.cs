using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Taktamir.Core.Domain._03.Users;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._4.Customers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TralnslateNarengi.Framework.Utilities;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._07.Suppliess;
using Microsoft.Extensions.Configuration;
using Taktamir.Core.Domain._08.Verifycodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;


namespace Taktamir.infra.Data.sql._01.Common
{
    public partial class AppDbContext : IdentityDbContext<User, Role, int>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //public AppDbContext()
        //{

        //}
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderJob> OrderJobs { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<Supplies> Supplies { get; set; }
        public virtual DbSet<Verifycode> Verifycodes { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasMany(p=>p.Specialties).WithOne(j=>j.User).HasForeignKey(s=>s.UserId);
            builder.Entity<User>().HasOne(u => u.Room).WithOne(r => r.User).HasForeignKey<Room>(r => r.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
             .HasOne(u => u.Wallet)
             .WithOne(w => w.User)
             .HasForeignKey<Wallet>(w => w.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            

            builder.Entity<Wallet>()
               .HasMany(w => w.Orders)
               .WithOne(o => o.Wallet)
               .HasForeignKey(o => o.WalletId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
             .HasOne(o => o.Wallet)
             .WithMany(w => w.Orders)
             .HasForeignKey(o => o.WalletId);


            builder.Entity<Job>()
                .HasOne(j => j.Customer)
                .WithMany(c => c.Jobs);


            builder.Entity<Room>().HasMany(r => r.Messages).WithOne(m => m.Room).HasForeignKey(m => m.RoomId);
            builder.Entity<Room>().Property(r => r.UsersId)
                .HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            base.OnModelCreating(builder);
                
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
       







        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity is null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
    }

}
