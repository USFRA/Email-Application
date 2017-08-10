namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;
    using System.ComponentModel;

    public partial class BroadcastingEmail
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("From")]
        public string FromEmail { get; set; }

        [Required]
        [DisplayName("To")]
        public string Receiver { get; set; }

        [StringLength(100)]
        public string ReadReceiptTo { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body Field is required.")]
        [AllowHtml]
        public string Body { get; set; }

        public DateTime Created { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        public bool Sent { get; set; }
    }
}
