using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("EthiopicWord")]
    public class EthiopicWord
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EthiopicWordID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int SourceWebPageID { get; set; }

        [ForeignKey("SourceWebPageID")]
        public WebPage SourceWebPage { get; set; }
    }
}
