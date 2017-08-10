namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Medium
    {
        //[Key]
        //public int Id { get; set; }

        //[Required, MaxLength(255), MinLength(1)]
        //[Display(Name = "Title")]
        //public string Title { get; set; }

        //public virtual MediaFile File { get; set; }
        //[Column("NaviNode_Id")]
        //public int? NaviNodeId { get; set; }

        //[MaxLength(100)]
        //public string CreatedBy { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //public System.DateTime Created { get; set; }

        //[MaxLength(100),]
        //public string ModifiedBy { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //public System.DateTime Modified { get; set; }   

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public int? NaviNode_Id { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime Modified { get; set; }

        public int? File_Id { get; set; }

        public virtual MediaFile MediaFile { get; set; }

    }
}
