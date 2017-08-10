namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EmailDbContext : DbContext
    {
        public EmailDbContext()
            : base("name=EmailDbContext")
        {
        }

        public virtual DbSet<BroadcastingEmail> BroadcastingEmails { get; set; }
        public virtual DbSet<Draft> Drafts { get; set; }
        public virtual DbSet<EmailTracking> EmailTrackings { get; set; }
        public virtual DbSet<FromEmail> FromEmails { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<MediaFile> MediaFiles { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<ToEmail> ToEmails { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FromEmail>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FromEmail>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<MediaFile>()
                .HasMany(e => e.Media)
                .WithOptional(e => e.MediaFile)
                .HasForeignKey(e => e.File_Id);

            modelBuilder.Entity<Template>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Template>()
                .Property(e => e.Html)
                .IsUnicode(false);

            modelBuilder.Entity<ToEmail>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ToEmail>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }

    public partial class CMSDbContext : DbContext
    {
        public CMSDbContext()
            : base("name=CMSDbContext")
        {
        }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaFile>()
                .HasMany(e => e.Media)
                .WithOptional(e => e.MediaFile)
                .HasForeignKey(e => e.File_Id);
        }

    }
}
