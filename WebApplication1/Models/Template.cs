namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;
    using System.ComponentModel;

    [Table("Template")]
    public partial class Template
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        [AllowHtml]
        public string Html { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("From")]
        public string FromEmail { get; set; }

        [Required]
        [DisplayName("To")]
        public string Receiver { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        public DateTime? Created { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

    }
}
