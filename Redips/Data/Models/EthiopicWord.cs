using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redips.Data.Models
{
    [Table("EthiopicWord", Schema = "web")]
    public class EthiopicWord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EthiopicWordID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int SourceWebPageID { get; set; }

        [ForeignKey("SourceWebPageID")]
        public WebPage SourceWebPage { get; set; }
    }
}
