using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("Word")]
    public class Word
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WordID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfEntry { get; set; }
        public int SourceSiteID { get; set; }

        [ForeignKey("SourceSiteID")]
        public Site SourceSite { get; set; }
    }
}
