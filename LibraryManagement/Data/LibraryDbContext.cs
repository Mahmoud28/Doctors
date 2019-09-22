using Seagull.Core.Data.Model;
using Seagull.Core.Models;
using Microsoft.EntityFrameworkCore;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Data.Model;
using System.Linq;
using ShortURLService.Models;
using Seagull.Doctors.Helper.URLShortner;

namespace Seagull.Core.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }
        public void Properties()
        {
            decimal Price;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MedicinesLookUp> MedicinesLookUp { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRoleMap> UserRoleMaps { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermUserRoleMap> PermUserRoleMap { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<EmailTemplate> Subscription { get; set; }
        public DbSet<EmailTransfer> EmailTransfer { set; get; }
        public DbSet<EmailTransferUser> EmailTransferUser { set; get; }
        public DbSet<URL> URLs { set; get; }
        public DbSet<Password> Password { set; get; }
        public DbSet<Slider> Slider { set; get; }
        public DbSet<PushNotification> PushNotification { get; set; }
        public DbSet<UsersNotification> UsersNotification { get; set; }
        public DbSet<PromoCodes> PromoCodes { get; set; }
        public DbSet<PromoCodeType> PromoCodeType { get; set; }
        public DbSet<CreateEventPlayCategorie> CreateEventPlayCategorie { get; set; }
        public DbSet<GustUserDevice> GustUserDevice { set; get; }
        public DbSet<Days> Dayss { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Pharmacy> Pharmacys { get; set; }
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<MedicationFailed> MedicationFaileds { get; set; }
        public DbSet<VisitType> VisitTypes { get; set; }
        public DbSet<Allergy> Allergys { get; set; }
        public DbSet<MedicationAllergyMap> MedicationAllergyMaps { get; set; }

        

        //public DbSet<PromoCodeType> PromoCodeType { get; set; }
        public DbSet<KNetData> KNetData { get; set; }
        public DbSet<WebViewUrl> WebViewUrl { get; set; }

        public DbSet<SeatsOrder> SeatsOrder { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<UserIPAddress> UserIPAddress { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<BloodType>().ToTable("BloodType");
            modelBuilder.Entity<MedicinesLookUp>().ToTable("MedicinesLookUp");

            modelBuilder.Entity<VisitType>().ToTable("VisitType");
            modelBuilder.Entity<Allergy>().ToTable("Allergy");
            modelBuilder.Entity<MedicationAllergyMap>().ToTable("MedicationAllergyMap");

            modelBuilder.Entity<GustUserDevice>().ToTable("GustUserDevice");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");

            modelBuilder.Entity<KNetData>().ToTable("KNetData");
            modelBuilder.Entity<SeatsOrder>().ToTable("SeatsOrder");

            modelBuilder.Entity<EmailTemplate>().ToTable("EmailTemplate");
            modelBuilder.Entity<PushNotification>().ToTable("PushNotification");


            modelBuilder.Entity<UsersNotification>().ToTable("UsersNotification");
            modelBuilder.Entity<PromoCodeType>().ToTable("PromoCodeType");
            modelBuilder.Entity<EmailTransfer>().ToTable("EmailTransfer");
            modelBuilder.Entity<EmailTransferUser>().ToTable("EmailTransferUser");
            modelBuilder.Entity<CreateEventPlayCategorie>().ToTable("CreateEventPlayCategorie");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Days>().ToTable("Days");
            modelBuilder.Entity<Medication>().ToTable("Medication");
            modelBuilder.Entity<Pharmacy>().ToTable("Pharmacy");
            modelBuilder.Entity<MedicationFailed>().ToTable("MedicationFailed");
            modelBuilder.Entity<PastSurgicalHistory>().ToTable("PastSurgicalHistory");
            modelBuilder.Entity<PastSurgicalHistoryImage>().ToTable("PastSurgicalHistoryImage");


            #region Many To Many Relation (UserRoleMap)

            modelBuilder.Entity<UserRoleMap>().ToTable("UserRoleMap")
             .HasKey(usrmap => new { usrmap.UserId, usrmap.UserRoleId });

            modelBuilder.Entity<UserRoleMap>()
                .HasOne(user => user.User)
                .WithMany(userMap => userMap.fk_UserRoleMap)
                .HasForeignKey(user => user.UserId);

            modelBuilder.Entity<UserRoleMap>()
                .HasOne(userrole => userrole.UserRole)
                .WithMany(userMap => userMap.fk_UserRoleMap)
                .HasForeignKey(userrole => userrole.UserRoleId);

            #endregion

            modelBuilder.Entity<Permission>().ToTable("Permission");

            #region Many To many Permision / UserRole

            modelBuilder.Entity<PermUserRoleMap>().ToTable("PermUserRoleMap")
             .HasKey(usrmap => new { usrmap.PermId, usrmap.UserRoleId });

            modelBuilder.Entity<PermUserRoleMap>()
                .HasOne(perm => perm.Permission)
                .WithMany(permMap => permMap.fk_PermUserRoleMap)
                .HasForeignKey(user => user.PermId);

            modelBuilder.Entity<PermUserRoleMap>()
                 .HasOne(role => role.UserRole)
                 .WithMany(roleMap => roleMap.fk_UserRolePermMap)
                 .HasForeignKey(user => user.UserRoleId);


            #endregion

            modelBuilder.Entity<SystemSetting>().ToTable("SystemSetting");

            modelBuilder.Entity<URL>().ToTable("URL");

            modelBuilder.Entity<Password>().ToTable("Password");

            modelBuilder.Entity<Slider>().ToTable("Slider");
            modelBuilder.Entity<PromoCodes>().ToTable("PromoCodes");
            modelBuilder.Entity<UserIPAddress>().ToTable("UserIPAddress");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");

        }
    }
}
