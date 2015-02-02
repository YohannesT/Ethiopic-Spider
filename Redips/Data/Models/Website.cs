using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redips.Data.Models
{
    [Table("Website", Schema = "web")]
    public class Website
    {
        [Key]
        public int WebsiteID { get; set; }
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public string Country { get; set; }
        public DateTime Date { get; set; }
        
        public string RobotsTxt { get; set; }
        public virtual Collection<WebPage> WebPages { get; set; } 
    }
}
