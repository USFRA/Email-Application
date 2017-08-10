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
    public class ToEmailsController : Controller
    {
        private EmailDbContext db = new EmailDbContext();

        // GET: ToEmails
        public ActionResult Index()
        {
            return View(db.ToEmails.ToList());
        }

        // GET: ToEmails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToEmail toEmail = db.ToEmails.Find(id);
            if (toEmail == null)
            {
                return HttpNotFound();
            }
            return View(toEmail);
        }

        // GET: ToEmails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToEmails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email")] ToEmail toEmail)
        {
            if (ModelState.IsValid)
            {
                db.ToEmails.Add(toEmail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toEmail);
        }

        // GET: ToEmails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToEmail toEmail = db.ToEmails.Find(id);
            if (toEmail == null)
            {
                return HttpNotFound();
            }
            return View(toEmail);
        }

        // POST: ToEmails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email")] ToEmail toEmail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toEmail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toEmail);
        }

        // GET: ToEmails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToEmail toEmail = db.ToEmails.Find(id);
            if (toEmail == null)
            {
                return HttpNotFound();
            }
            return View(toEmail);
        }

        // POST: ToEmails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToEmail toEmail = db.ToEmails.Find(id);
            db.ToEmails.Remove(toEmail);
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
