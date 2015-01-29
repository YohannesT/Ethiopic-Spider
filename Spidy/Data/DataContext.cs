using System.Data.Entity;
using Spider.Data.Models;

namespace Spider.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("data")
        {

        }

        public DbSet<Website> Websites { get; set; }
        public DbSet<WebPage> WebPages { get; set; }
        public DbSet<EthiopicWord> EthiopicWords { get; set; }
        public DbSet<SeedWebsite> SeedSites { get; set; }
    }
}
