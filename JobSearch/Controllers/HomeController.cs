using JobSearch.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }
    public ActionResult Details(int JobId)
        {
            var job = db.Jobs.Find(JobId);
            if (job == null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = JobId;
            return View(job);
        }
        [Authorize]
        public ActionResult Apply()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Apply(string Message)
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];
            var check = db.ApplyForJobs.Where(a => a.JobId==JobId &&  a.UserId==UserId).ToList();
            if (check.Count < 1)
            {
                var job = new ApplyForJob();
                job.UserId = UserId;
                job.JobId = JobId;
                job.Message = Message;
                job.ApplyDate = DateTime.Now;
                db.ApplyForJobs.Add(job);
                db.SaveChanges();
                ViewBag.Result = "Application sent successfully";
            }
            else
            {
                ViewBag.Result = "Sorry!You already applied for the job";
            }
            return View();
        }
        // actions related to jobs applied by one user 
        [Authorize]
        public ActionResult GetJobsByUser()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = db.ApplyForJobs.Where(a => a.UserId == UserId);
            return View(Jobs.ToList());
        }
        
        // dtalis for job that a user applied using applyforjob id
        
        public ActionResult DetailsOfJob(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
           
            return View(job);
        }
        

        // GET: ApplyForJob/Edit
        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }
        // POST: ApplyForJob/Edit
        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    job.ApplyDate = DateTime.Now;

                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("GetJobsByUser");

                }
                return View(job);
            }
            catch
            {
                return View();
            }
        }
        // GET: ApplyForJob/Delete
        public ActionResult Delete(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: ApplyForJob/Delete
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {
            try
            {
                // TODO: Add delete logic here
                var jobApplied = db.ApplyForJobs.Find(job.Id);
                db.ApplyForJobs.Remove(jobApplied);
                db.SaveChanges();
                return RedirectToAction("GetJobsByUser");
            }
            catch
            {
                return View(job);
            }
        }
        [Authorize]
        public ActionResult GetJobsByPublishers()
        {
            var UserID = User.Identity.GetUserId();
            //getting the  applied jobs that the user published 
            var Jobs = from app in db.ApplyForJobs
                       join job in db.Jobs
                       on app.JobId equals job.Id
                       where job.User.Id == UserID
                       select app;

            var grouped = from j in Jobs
                          group j by j.job.JobTitle
                        into gr
                          select new JobsViewModel
                          {
                              JobTitle = gr.Key,
                              Items = gr
                          };

            return View(grouped.ToList());
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            var mail = new MailMessage();
            var loginInfo = new NetworkCredential("aidtunisie123@gmail.com","joujou12");
            mail.From = new MailAddress(contact.Email);
            mail.To.Add(new MailAddress("aidtunisie123@gmail.com"));
            mail.Subject = contact.Subject;
            mail.IsBodyHtml = true;
            string body = "Sender : " + contact.Name + "<br />" +
                "Sender's e-mail : " + contact.Email + "<br />" +
                "Subject : " + contact.Subject + "<br />"
                + "Content : <b>" + contact.Message+"</b>";
            mail.Body = body;
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(mail);
            return RedirectToAction("Index");
        }
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string searchName)
        {
            var result = db.Jobs.Where(a => a.JobTitle.Contains(searchName)
              ||  a.JobContent.Contains(searchName)
            || a.Category.CategoryName.Contains(searchName)
            || a.Category.CategoryDescription.Contains(searchName)).ToList();
            return View(result);
        }
    }
}