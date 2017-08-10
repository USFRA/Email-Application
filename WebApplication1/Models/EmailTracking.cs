namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmailTracking
    {
        public int Id { get; set; }

        public int MessageId { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        public DateTime Created { get; set; }
    }
}
