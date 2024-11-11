using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Entities.Roles;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.Users;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistence.EF.DataContext
{
    public class EFDataContext : DbContext
    {
        public EFDataContext(DbContextOptions<EFDataContext> options) : base(options)
        {
        }
        public EFDataContext(string connectionString)
        : this(new DbContextOptionsBuilder<EFDataContext>().UseSqlServer(connectionString).Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(UserEntityMap).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanTemplate> LoanTemplates { get; set; }
        public DbSet<Installment> Installments { get; set; }
    }
}
