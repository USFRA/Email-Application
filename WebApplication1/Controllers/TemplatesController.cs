using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Security;

namespace WebApplication1.Controllers
{
    [UserRoleAuthorize(Roles = "Sender")]
    public class TemplatesController : Controller
    {
        private EmailDbContext db = new EmailDbContext();

        // GET: Templates
        public ActionResult Index()
        {
            bool IsAdmin = db.Users.Any(u => u.Name == User.Identity.Name && u.Roles.Contains("Admin"));

            List<Template> model = new List<Template>();

            var data = (from a in db.Templates
                        join b in db.FromEmails on a.FromEmail equals b.Email
                        join c in db.ToEmails on a.Receiver equals c.Email
                        select new
                        {
                            ID = a.ID,
                            Name = a.Name,
                            FromEmail = b.Name,
                            Receiver = c.Name,
                            Subject = a.Subject,
                            Body = a.Html,
                            CreatedBy = a.CreatedBy,
                            Created = a.Created
                        }).ToList();

            if (!IsAdmin)
                data = data.Where(x => x.CreatedBy == System.Web.HttpContext.Current.User.Identity.Name.ToString()).ToList();

            foreach (var d in data)
            {
                model.Add(new Template
                {
                    ID = d.ID,
                    Name = d.Name,
                    FromEmail = d.FromEmail,
                    Receiver = d.Receiver,
                    Subject = d.Subject,
                    Html = d.Body,
                    CreatedBy = d.CreatedBy,
                    Created = d.Created
                });
            }

            //if (IsAdmin)
            //    model = db.Templates.OrderByDescending(x => x.Created).ToList();
            //else
            //    model = db.Templates.Where(x => x.CreatedBy.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).OrderByDescending(x => x.Created).ToList();

            return View(model.OrderByDescending(x => x.Created));
        }

        // GET: Templates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }
            return View(template);
        }

        // GET: Templates/Create
        public ActionResult Create()
        {
            var sender = db.FromEmails.ToList();
            ViewBag.Senders = sender;

            var receiver = db.ToEmails.ToList();
            ViewBag.ToEmail = receiver;

            return View();
        }

        // POST: Templates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Html,CreatedBy,FromEmail,Receiver,Subject")] Template template)
        {
            if (ModelState.IsValid)
            {
                template.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                template.Created = DateTime.Now;
                db.Templates.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

            }
            return View(template);
        }

        // GET: Templates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }

            var sender = db.FromEmails.ToList();
            ViewBag.Senders = sender;

            var receiver = db.ToEmails.ToList();
            ViewBag.ToEmail = receiver;


            return View(template);
        }

        // POST: Templates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Html,CreatedBy,FromEmail,Receiver,Subject")] Template template)
        {
            if (ModelState.IsValid)
            {
                template.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                template.Created = DateTime.Now;
                db.Entry(template).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        // GET: Templates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }
            return View(template);
        }

        // POST: Templates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Template template = db.Templates.Find(id);
            db.Templates.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Popup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }
            return PartialView("Popup", template);

        }
    }
}
