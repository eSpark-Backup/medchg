using MEDCHGClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MEDCHGClassLibrary.DbContext
{
    public class MedChgContext : IdentityDbContext<ApplicationUser>
    {
        public MedChgContext()
        {
        }
        public MedChgContext(DbContextOptions<MedChgContext> options)
       : base(options)
        {
        }
        public virtual DbSet<DiagnosticCode> DiagnosticCodes { get; set; }
        public virtual DbSet<ChargeCode> ChargeCodes { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientTempCode> PatientTempCodes { get; set; }
        public virtual DbSet<PatientVisitHistory> PatientVisitHistory { get; set; }
        public virtual DbSet<PatientVisitHistoryDetailList> PatientVisitHistoryDetailLists { get; set; }
        public virtual DbSet<UsersHospital> UsersHospitals { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersChargeCode> UsersChargeCodes { get; set; }
        public virtual DbSet<UsersDiagnosticCode> UsersDiagnosticCodes { get; set; }
        public virtual DbSet<ProviderTimerSetting> ProviderTimerSettings { get; set; }
        public virtual DbSet<DeleteLog> DeleteLogs { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
             .Property(u => u.ProviderName)
             .HasMaxLength(256);

            modelBuilder.Entity<Room>().Ignore(p => p.RoomCode).Ignore(p => p.RoomNo).Ignore(p => p.Patient);
            modelBuilder.Entity<Floor>().Ignore(p => p.RoomRange1).Ignore(p => p.RoomRange2).Ignore(p => p.Room).Ignore(p => p.AlreadyCreated);
            modelBuilder.Entity<Patient>().Ignore(p => p.Year).Ignore(p => p.Month).Ignore(p => p.Day).Ignore(p => p.Name);
            modelBuilder.Entity<Floor>().Property(p => p.Id).IsConcurrencyToken();
        }
    }
}
