using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("SeedSite")]
    public class SeedSite
    {
        [Key]
        public int SeedSiteID { get; set; }
        public string URL { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisited { get; set; }
    }
}
