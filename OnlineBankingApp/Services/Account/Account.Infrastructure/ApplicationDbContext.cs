using Account.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AccountOwner = Account.Core.Domain.Account;

namespace Account.Infrastructure
{
    [Serializable]
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<AccountOwner> Accounts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("MsSqlConnection");
            optionsBuilder.UseSqlServer(connectionString).UseLazyLoadingProxies();
        }
    }
}