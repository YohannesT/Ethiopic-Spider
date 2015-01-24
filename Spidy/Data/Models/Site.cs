using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spider.Data.Models
{
    [Table("Site")]
    public class Site
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SiteID { get; set; }
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public string Country { get; set; }
        
        public long StreamLength { get; set; }
        public long HtmlCharLength { get; set; }

        public DateTime VisitedDate { get; set; }
        public int NumberOfVisits { get; set; }

        public int? NavigatedFromSiteID { get; set; }

        [ForeignKey("NavigatedFromSiteID")]
        public virtual Site ParentSite { get; set; }

        public virtual Collection<Site> ChildSites { get; set; } 
    }
}
