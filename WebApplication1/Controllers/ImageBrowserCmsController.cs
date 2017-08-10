using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WebApplication1.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using WebApplication1.Security;

namespace WebApplication1.Controllers
{
    [UserRoleAuthorize(Roles = "Sender")]
    public class ImageBrowserCmsController : Controller
    {
        private const int ThumbnailHeight = 80;
        private const int ThumbnailWidth = 80;

        //private IRepository<Medium> _mediaRepo;

        private CMSDbContext db = new CMSDbContext();

        private EmailDbContext db2 = new EmailDbContext();

        //public ImageBrowserCmsController(IRepository<Medium> mediaRepo)
        public ImageBrowserCmsController()
        {
            //this._mediaRepo = mediaRepo;
        }

        public ActionResult Index(string type, string path)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var imageName = NodeHelper.GetNameFromPath(path);

            bool IsAdmin = db2.Users.Any(u => u.Name == User.Identity.Name && u.Roles.Contains("Admin"));

            List<Medium> media = new List<Medium>();

            if (IsAdmin)
                media = db.Media.Where(e => e.NaviNode_Id == nodeId).ToList();
            else
                media = db.Media.Where(e => e.NaviNode_Id == nodeId && e.CreatedBy.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).ToList();

            ViewBag.NodeId = nodeId;
            return PartialView(media);
            //return View();
        }

        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        public ActionResult Thumbnail(string path)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var imageName = NodeHelper.GetNameFromPath(path);
            var media = db.Media.Where(e => e.NaviNode_Id == nodeId && e.Title.Equals(imageName, StringComparison.InvariantCultureIgnoreCase)).Select(e => e).ToList()[0];
            var ms = new MemoryStream(media.MediaFile.FileContent);
            var image = Image.FromStream(ms);
            var thumbnail = ResizeImage(image, ThumbnailWidth, ThumbnailHeight);
            var thumbnailStream = new MemoryStream();
            thumbnail.Save(thumbnailStream, image.RawFormat);
            return File(thumbnailStream.ToArray(), media.MediaFile.FileType, media.MediaFile.FileName);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.Clamp);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public ActionResult Delete(string path)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var imageName = NodeHelper.GetNameFromPath(path);
            var m = db.Media.SingleOrDefault(e => e.NaviNode_Id == nodeId && e.Title.Equals(imageName, StringComparison.InvariantCultureIgnoreCase));
            if (m != null)
            {
                db.Media.Remove(m);
                db.SaveChanges();
            }
            
            return Json(new
            {
                status = "success"
            });
        }

        public ActionResult Destroy(string path, string name, string type)
        {
            if (!String.IsNullOrEmpty(type) && type.Equals("f", StringComparison.InvariantCultureIgnoreCase))
            {
                var nodeId = NodeHelper.GetNodeIdFromPath(path);
                var m = db.Media.SingleOrDefault(e => e.NaviNode_Id == nodeId && e.Title.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (m != null)
                {
                    db.Media.Remove(m);
                    db.SaveChanges();
                }
            }

            return Json(new
            {
                status = "success"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">{sectionId}/{folders}</param>
        /// <returns></returns>
        public JsonResult Read(string path)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var images = db.Media.Where(e => e.NaviNode_Id == nodeId).Select(e => new
            {
                name = e.Title,
                type = "f",
                size = e.MediaFile.FileSize
            });

            return Json(images, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload(string path, HttpPostedFileBase file)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var created = DateTime.Now;
            var createdBy = HttpContext.User.Identity.Name;

            var mediaFile = new MediaFile
            {
                Created = created,
                CreatedBy = createdBy,
                FileName = Path.GetFileName(file.FileName),
                FileSize = file.ContentLength,
                FileType = file.ContentType,
                Modified = created,
                ModifiedBy = createdBy
            };
            try
            {
                using (MemoryStream target = new MemoryStream(file.ContentLength))
                {
                    file.InputStream.CopyTo(target);
                    mediaFile.FileContent = target.ToArray();
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "error",
                    message = e.Message
                });
            }

            var existedFiles = db.Media.Where(e => e.NaviNode_Id == nodeId && e.Title == file.FileName);
            if (existedFiles.Any())
            {
                var m = existedFiles.First();
                m.Modified = created;
                m.ModifiedBy = createdBy;
                m.MediaFile = mediaFile;
            }
            else
            {
                var m = new Medium
                {
                    Created = created,
                    CreatedBy = createdBy,
                    MediaFile = mediaFile,
                    Modified = created,
                    ModifiedBy = createdBy,
                    NaviNode_Id = nodeId,
                    Title = mediaFile.FileName
                };
                db.Media.Add(m);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "error",
                    message = e.Message
                });
            }
            return Json(new
            {
                size = mediaFile.FileSize,
                name = mediaFile.FileName,
                type = "f"
            });
        }
    }
}