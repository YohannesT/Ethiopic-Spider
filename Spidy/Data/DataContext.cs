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

        public DbSet<Site> Sites { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Paragraph> Paragraphs { get; set; }
        public DbSet<SeedSite> SeedSites { get; set; }
    }
}
