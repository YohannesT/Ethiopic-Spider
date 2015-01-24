using System.Data.Entity;
using Spider.Data.Models;

namespace Spider.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
            //    : base(@"Data Source = 192.168.2.58; Initial Catalog = DEV_SYSTEMHEALTH; UID=hcmis;pwd=hcmis;")
            : base("repo")
        {

        }
        public DbSet<Website> Websites { get; set; }
        public DbSet<WebPage> Sites { get; set; }
        public DbSet<WebContent> Paragraphs { get; set; }
        public DbSet<EthiopicWord> EthiopicWords { get; set; }
        public DbSet<SeedWebsite> SeedSites { get; set; }
    }
}
