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
using System.Net.Mail;


namespace WebApplication1.Controllers
{
    [UserRoleAuthorize(Roles = "Sender")]
    public class EmailController : Controller
    {
        private EmailDbContext db = new EmailDbContext();

        // GET: Email
        public ActionResult Index()
        {
            bool IsAdmin = db.Users.Any(u => u.Name == User.Identity.Name && u.Roles.Contains("Admin"));

            List<BroadcastingEmail> model = new List<BroadcastingEmail>();

            var data = (from a in db.BroadcastingEmails
                     join b in db.FromEmails on a.FromEmail equals b.Email
                     join c in db.ToEmails on a.Receiver equals c.Email
                     where a.Sent == false
                     select new
                     {
                         Id = a.Id,
                         FromEmail = b.Name,
                         Receiver = c.Name,
                         Subject = a.Subject,
                         Body = a.Body,
                         CreatedBy = a.CreatedBy,
                         Created = a.Created
                     }).ToList();

            if (!IsAdmin)
                data = data.Where(x => x.CreatedBy == System.Web.HttpContext.Current.User.Identity.Name.ToString()).ToList();

            foreach (var d in data)
            {
                model.Add(new BroadcastingEmail
                {
                    Id = d.Id,
                    FromEmail = d.FromEmail,
                    Receiver = d.Receiver,
                    Subject = d.Subject,
                    Body = d.Body,
                    CreatedBy = d.CreatedBy,
                    Created = d.Created
                });
            }
            
            return View(model.OrderByDescending(x => x.Created));
        }

        public ActionResult Templates()
        {
            bool IsAdmin = db.Users.Any(u => u.Name == User.Identity.Name && u.Roles.Contains("Admin"));

            List<Template> model = new List<Template>();

            if (IsAdmin)
                model = db.Templates.ToList();
            else
                model = db.Templates.Where(x => x.CreatedBy.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).ToList();

            return View(model);
        }

        public ActionResult GetTemplateData(int templateId)
        {
            var TextareaHtml = db.Templates.Where(x => x.ID == templateId).Select(x => x.Html);
            return Json(TextareaHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SentEmails()
        {
            bool IsAdmin = db.Users.Any(u => u.Name == User.Identity.Name && u.Roles.Contains("Admin"));

            List<BroadcastingEmail> model = new List<BroadcastingEmail>();

            var data = (from a in db.BroadcastingEmails
                        join b in db.FromEmails on a.FromEmail equals b.Email
                        join c in db.ToEmails on a.Receiver equals c.Email
                        where a.Sent == true
                        select new
                        {
                            Id = a.Id,
                            FromEmail = b.Name,
                            Receiver = c.Name,
                            Subject = a.Subject,
                            Body = a.Body,
                            CreatedBy = a.CreatedBy,
                            Created = a.Created
                        }).ToList();

            if (!IsAdmin)
                data = data.Where(x => x.CreatedBy == System.Web.HttpContext.Current.User.Identity.Name.ToString()).ToList();

            foreach (var d in data)
            {
                model.Add(new BroadcastingEmail
                {
                    Id = d.Id,
                    FromEmail = d.FromEmail,
                    Receiver = d.Receiver,
                    Subject = d.Subject,
                    Body = d.Body,
                    CreatedBy = d.CreatedBy,
                    Created = d.Created
                });
            }

            return View(model.OrderByDescending(x => x.Created));
        }

        // GET: Email/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BroadcastingEmail broadcastingEmail = db.BroadcastingEmails.Find(id);
            if (broadcastingEmail == null)
            {
                return HttpNotFound();
            }
            return View(broadcastingEmail);
        }

        

        // GET: Email/Create
        public ActionResult Create()
        {
            var sender = db.FromEmails.ToList();
            ViewBag.Senders = sender;

            var receiver = db.ToEmails.ToList();
            ViewBag.ToEmail = receiver;

            ViewBag.BodyHtml = "";

            BroadcastingEmail broadcastingemail = new BroadcastingEmail();

            if (!String.IsNullOrEmpty(Request.QueryString["templateId"]))
            {
                int templateId = Convert.ToInt32(Request.QueryString["templateId"]);
                //var TextareaHtml = db.Templates.Where(x => x.ID == templateId).Select(x => x.Html).FirstOrDefault();
                var templateData = db.Templates.Where(x => x.ID == templateId).FirstOrDefault();
                ViewBag.BodyHtml = "";

                broadcastingemail.Body = templateData.Html;
                broadcastingemail.FromEmail = templateData.FromEmail;
                broadcastingemail.Receiver = templateData.Receiver;
                broadcastingemail.Subject = templateData.Subject;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["emailId"]))
            {
                //int templateId = Convert.ToInt32(Request.QueryString["templateId"]);
                //var TextareaHtml = db.Templates.Where(x => x.ID == templateId).Select(x => x.Html).FirstOrDefault();
                //ViewBag.BodyHtml = "";


                //broadcastingemail.Body = TextareaHtml;
                //broadcastingemail.FromEmail = "s.pandey.ctr@dot.gov";

                int emailId = Convert.ToInt32(Request.QueryString["emailId"]);
                broadcastingemail = db.BroadcastingEmails.Find(emailId);
            }
            
            

            //var model = new FromEmail();
            //model.Senders = new List<WebApplication1.Models.FromEmail>();

            return View(broadcastingemail);
        }

        // POST: Email/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FromEmail,Receiver,ReadReceiptTo,Subject,Body,Created,CreatedBy,Sent")] BroadcastingEmail broadcastingEmail)
        {
            if (ModelState.IsValid)
            {
                broadcastingEmail.Body = HttpUtility.HtmlDecode(broadcastingEmail.Body);
                broadcastingEmail.Created = DateTime.Now;
                broadcastingEmail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                broadcastingEmail.Sent = true;
                db.BroadcastingEmails.Add(broadcastingEmail);
                db.SaveChanges();
                SendEmail(broadcastingEmail);
                return RedirectToAction("SentEmails");
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

                var sender = db.FromEmails.ToList();
                ViewBag.Senders = sender;

                var receiver = db.ToEmails.ToList();
                ViewBag.ToEmail = receiver;


            }

            return View("SentEmails", broadcastingEmail);
        }

        
        public ActionResult Draft([Bind(Include = "Id,FromEmail,Receiver,ReadReceiptTo,Subject,Body,Created,CreatedBy,Sent")] BroadcastingEmail broadcastingEmail)
        {
            if (ModelState.IsValid)
            {
                broadcastingEmail.Body = HttpUtility.HtmlDecode(broadcastingEmail.Body);
                broadcastingEmail.Created = DateTime.Now;
                broadcastingEmail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                broadcastingEmail.Sent = false;

                if(broadcastingEmail.Id > 0)
                {
                    db.Entry(broadcastingEmail).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.BroadcastingEmails.Add(broadcastingEmail);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            }

            return View(broadcastingEmail);
        }

        [ValidateInput(false)]
        public JsonResult SendTest(string body, string subject)
        {
            try
            { 
            BroadcastingEmail broadcastingEmail = new BroadcastingEmail();
            //string tempReceiver = broadcastingEmail.Receiver;
            //string tempFromEmail = broadcastingEmail.FromEmail;
            broadcastingEmail.Receiver = System.Web.HttpContext.Current.User.Identity.Name.ToString().Substring(6) + "@dot.gov";
            broadcastingEmail.FromEmail = System.Web.HttpContext.Current.User.Identity.Name.ToString().Substring(6) + "@dot.gov";
            broadcastingEmail.Body = body;
            broadcastingEmail.Subject = subject;
            broadcastingEmail.Created = DateTime.Now;
            broadcastingEmail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();

            SendEmail(broadcastingEmail);

            return Json(new
            {
                status = "Test Email Sent.",

            }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    status = "Error in Sending Test Email.",

                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TemplateSave([Bind(Include = "Id,FromEmail,Receiver,ReadReceiptTo,Subject,Body,Created,CreatedBy,Sent")] BroadcastingEmail broadcastingEmail)
        {
            Template template = new Template();

            if (ModelState.IsValid)
            {
                template.Html = HttpUtility.HtmlDecode(broadcastingEmail.Body);
                template.Receiver = broadcastingEmail.Receiver;
                template.FromEmail = broadcastingEmail.FromEmail;
                template.Subject = broadcastingEmail.Subject;
                template.Name = broadcastingEmail.Subject;
                template.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                db.Templates.Add(template);
                db.SaveChanges();

                return RedirectToAction("Index","Templates");
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            }

            return View(broadcastingEmail);
        }

        // GET: Email/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BroadcastingEmail broadcastingEmail = db.BroadcastingEmails.Find(id);
            if (broadcastingEmail == null)
            {
                return HttpNotFound();
            }

            var sender = db.FromEmails.ToList();
            ViewBag.Senders = sender;

            var receiver = db.ToEmails.ToList();
            ViewBag.ToEmail = receiver;


            return View(broadcastingEmail);
        }

        // POST: Email/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FromEmail,Receiver,ReadReceiptTo,Subject,Body,Created,CreatedBy")] BroadcastingEmail broadcastingEmail)
        {
            if (ModelState.IsValid)
            {
                broadcastingEmail.Body = HttpUtility.HtmlDecode(broadcastingEmail.Body);
                broadcastingEmail.Created = DateTime.Now;
                broadcastingEmail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                broadcastingEmail.Sent = true;
                db.Entry(broadcastingEmail).State = EntityState.Modified;
                db.SaveChanges();
                SendEmail(broadcastingEmail);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

                var sender = db.FromEmails.ToList();
                ViewBag.Senders = sender;

                var receiver = db.ToEmails.ToList();
                ViewBag.ToEmail = receiver;


            }
            return View(broadcastingEmail);
        }

        // GET: Email/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BroadcastingEmail broadcastingEmail = db.BroadcastingEmails.Find(id);
            if (broadcastingEmail == null)
            {
                return HttpNotFound();
            }
            return View(broadcastingEmail);
        }

        // POST: Email/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BroadcastingEmail broadcastingEmail = db.BroadcastingEmails.Find(id);
            db.BroadcastingEmails.Remove(broadcastingEmail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SendEmail(BroadcastingEmail mail)
        {

            if (string.IsNullOrWhiteSpace(mail.FromEmail))
            {
                throw new Exception("Email-From cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(mail.Receiver))
            {
                throw new Exception("Email-To cannot be empty.");
            }

            SmtpClient smtpClient = new SmtpClient();
            MailMessage message = new MailMessage();

            if (!string.IsNullOrWhiteSpace(mail.ReadReceiptTo))
            {
                message.Headers.Add("Disposition-Notification-To", mail.ReadReceiptTo);
            }

            MailAddress fromAddress = new MailAddress(mail.FromEmail);

            message.From = fromAddress;

            message.Subject = mail.Subject;

            message.IsBodyHtml = true;

            string baseurl = System.Web.HttpContext.Current.Session["AppURL"].ToString();

            //baseurl = baseurl.Substring(0, baseurl.LastIndexOf('/') + 1); //HttpContext.Current.Session["AppURL"]

            message.Body = mail.Body;

            message.Body = mail.Body +
                           @"<div><img alt='' width='1' height='1' src='"
                            + baseurl
                            + "Image/"
                            + mail.Id
                            + "\' /></div>";

            foreach (var address in mail.Receiver.Split(';'))
            {
                message.Bcc.Add(new MailAddress(address, ""));
            }

            smtpClient.Send(message);
        }

        [NoCacheAttribute]
        public ActionResult Image(int id = 0)
        {
            if (id != 0)
            {
                var email = db.BroadcastingEmails.SingleOrDefault(e => e.Id == id);

                if (email != null)
                {
                    EmailTracking emailTracking = new EmailTracking();

                    emailTracking.MessageId = id;
                    emailTracking.IP = Request.UserHostAddress;
                    emailTracking.Created = DateTime.Now;

                    db.EmailTrackings.Add(emailTracking);

                    db.SaveChanges();
                }
            }

            var dir = Server.MapPath("~/Images");
            var path = System.IO.Path.Combine(dir, "1x1_pixel.jpg");
            return base.File(path, "image/jpeg");
        }

        [HttpGet]
        public ActionResult Popup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BroadcastingEmail broadcastingEmail = db.BroadcastingEmails.Find(id);
            if (broadcastingEmail == null)
            {
                return HttpNotFound();
            }

            //var sender = db.FromEmails.ToList();
            //ViewBag.Senders = sender;

            //var receiver = db.ToEmails.ToList();
            //ViewBag.ToEmail = receiver;

            return PartialView("Popup", broadcastingEmail);

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
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}
