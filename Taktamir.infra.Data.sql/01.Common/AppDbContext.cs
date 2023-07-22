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

namespace Taktamir.infra.Data.sql._01.Common
{
    public partial class AppDbContext : IdentityDbContext<User, Role, int>
    {

        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasMany(p=>p.Specialties).WithOne(j=>j.User).HasForeignKey(s=>s.UserId);
            builder.Entity<User>()
             .HasOne(u => u.Wallet)
             .WithOne(w => w.User)
             .HasForeignKey<Wallet>(w => w.UserId);

            base.OnModelCreating(builder);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder  )
        {

           // optionsBuilder.UseSqlite("Data Source=LocalDatabase.db");
        }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Supplies> Supplies { get; set; }
        public virtual DbSet<Verifycode> Verifycodes  { get; set; }
        public virtual DbSet<Specialty> Specialties  { get; set; }


        





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
