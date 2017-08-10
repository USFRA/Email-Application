namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MediaFile
    {
    //    public MediaFile()
    //    {
    //        Media = new HashSet<Medium>();
    //    }

    //    public int Id { get; set; }

    //    [StringLength(255)]
    //    public string FileName { get; set; }

    //    [StringLength(31)]
    //    public string FileType { get; set; }

    //    public int FileSize { get; set; }

    //    public byte[] FileContent { get; set; }

    //    [StringLength(100)]
    //    public string CreatedBy { get; set; }

    //    public DateTime Created { get; set; }

    //    [StringLength(100)]
    //    public string ModifiedBy { get; set; }

    //    public DateTime Modified { get; set; }

    //    public virtual ICollection<Medium> Media { get; set; }
    //}

        public MediaFile()
        {
            Media = new HashSet<Medium>();
        }

        public int Id { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(31)]
        public string FileType { get; set; }

        public int FileSize { get; set; }

        public byte[] FileContent { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime Modified { get; set; }

        public virtual ICollection<Medium> Media { get; set; }
    }
}
