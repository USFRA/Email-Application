namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class Draft
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string FromEmail { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; }
    }
}
