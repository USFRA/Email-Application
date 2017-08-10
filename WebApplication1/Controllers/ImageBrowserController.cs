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

namespace WebApplication1.Controllers
{
    public class ImageBrowserController : Controller
    {
        // GET: ImageBrowser
        public ActionResult Index()
        {
            return View();
        }

        //private IRepository<Medium> _mediaRepo;

        private CMSDbContext db = new CMSDbContext();

        //public ImageBrowserController(IRepository<Medium> mediaRepo)
        //public ImageBrowserController(Medium media)
        //{
        //    //this._mediaRepo = mediaRepo;
        //}

        public ActionResult Image(string path)
        {
            var nodeId = NodeHelper.GetNodeIdFromPath(path);
            var imageName = NodeHelper.GetNameFromPath(path);
            var media = db.Media.Where(e => e.NaviNode_Id == nodeId && e.Title.Equals(imageName, StringComparison.InvariantCultureIgnoreCase)).Select(e => e).ToList()[0];
            var ms = new MemoryStream(media.MediaFile.FileContent);
            return File(ms, media.MediaFile.FileType);
        }
    }
}