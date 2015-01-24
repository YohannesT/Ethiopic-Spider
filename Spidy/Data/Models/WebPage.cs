using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("WebPage")]
    public class WebPage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WebPageID { get; set; }
        public string Url { get; set; }
         
        public long StreamLength { get; set; }
       
        public DateTime Date { get; set; }
        public int NumberOfVisits { get; set; }

        public int WebsiteID { get; set; }
        public int? NavigatedFromWebPageID { get; set; }


        public string HtmlContent { get; set; }
        public string TextContent { get; set; }

        [ForeignKey("NavigatedFromWebPageID")]
        public virtual WebPage ParentSite { get; set; }

        [ForeignKey("WebsiteID")]
        public virtual Website Website { get; set; }
        public virtual Collection<WebPage> ChildSites { get; set; } 
    }
}
