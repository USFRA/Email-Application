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
    [UserRoleAuthorize(Roles = "Admin")]
    public class FromEmailsController : Controller
    {
        private EmailDbContext db = new EmailDbContext();

        // GET: FromEmails
        public ActionResult Index()
        {
            return View(db.FromEmails.ToList());
        }

        // GET: FromEmails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FromEmail fromEmail = db.FromEmails.Find(id);
            if (fromEmail == null)
            {
                return HttpNotFound();
            }
            return View(fromEmail);
        }

        // GET: FromEmails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FromEmails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email")] FromEmail fromEmail)
        {
            if (ModelState.IsValid)
            {
                db.FromEmails.Add(fromEmail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fromEmail);
        }

        // GET: FromEmails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FromEmail fromEmail = db.FromEmails.Find(id);
            if (fromEmail == null)
            {
                return HttpNotFound();
            }
            return View(fromEmail);
        }

        // POST: FromEmails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email")] FromEmail fromEmail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fromEmail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fromEmail);
        }

        // GET: FromEmails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FromEmail fromEmail = db.FromEmails.Find(id);
            if (fromEmail == null)
            {
                return HttpNotFound();
            }
            return View(fromEmail);
        }

        // POST: FromEmails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FromEmail fromEmail = db.FromEmails.Find(id);
            db.FromEmails.Remove(fromEmail);
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
    }
}
