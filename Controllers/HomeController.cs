using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Royal_Project_3.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Royal_Project_3.Controllers
{
    public class HomeController : Controller
    {
        // Get : Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        
        public ActionResult Contact()
        {
            return View();
        }
        
    }
}