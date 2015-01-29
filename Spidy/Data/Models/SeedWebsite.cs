using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("SeedWebsite", Schema = "web")]
    public class SeedWebsite
    {
        [Key]
        public int SeedWebsiteID { get; set; }
        public string URL { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisited { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int CrawlDelayInMinutes { get; set; }
        public int VisitCount { get; set; }
    }
}
