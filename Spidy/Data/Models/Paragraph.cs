using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("Paragraph")]
    public class Paragraph
    {
        [Key]
        public int ParagraphID { get; set; }
        public string Value { get; set; }
        public DateTime DateOfEntry { get; set; }
        public int SourceSiteID { get; set; }

        [ForeignKey("SourceSiteID")]
        public Site SourceSite { get; set; }
    }
}
