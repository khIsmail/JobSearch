using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace JobSearch.Models
{
    public class Job
    {
        public int Id { get; set; }
        [Display(Name="Job Name")]
        public string JobTitle { get; set; }
        [AllowHtml]
        [Display(Name ="Job Description")]
        public string JobContent { get; set; }
        [Display(Name ="Image")]
        public string JobImage { get; set; }
        [Display(Name ="Job Category")]
        public int CategoryId { get; set; }
        public string UserID { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}