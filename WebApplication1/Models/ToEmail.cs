namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("ToEmail")]
    public partial class ToEmail
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [AllowHtml]
        public string Email { get; set; }
    }
}
