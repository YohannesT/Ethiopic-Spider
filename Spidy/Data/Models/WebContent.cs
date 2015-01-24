using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("WebContent")]
    public class WebContent
    {
        [Key]
        public int WebContentID { get; set; }
        public string HtmlContent { get; set; }
        public string TextContent { get; set; }
        public DateTime Date { get; set; }
        public int SourceWebPageID { get; set; }

        [ForeignKey("SourceWebPageID")]
        public WebPage SourceWebPage { get; set; }
    }
}
