using Royal_Project_3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Web.UI.DataVisualization.Charting;
using System.Text;
using System.Drawing;

namespace Royal_Project_3.Controllers
{
    public class AdminController : Controller
    {
        RoyalProjectEntities db = new RoyalProjectEntities();

        public ActionResult ChartFromEF()
        {
            var data = db.GetChart().ToList();
            var chart = new Chart
            {
                Width = 800,
                Height = 450,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            chart.Titles.Add("Total Product Sale");
            chart.Titles[0].Font = new Font("Arial", 16f);
            chart.ChartAreas[0].AxisX.Title = "Order Date";
            chart.ChartAreas[0].AxisY.Title = "No. of items purchsed";
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 12f);
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 12f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 10f);
            var series = new Series();
            foreach(var item in data)
            {
                
                series.Points.AddXY(item.pro_name, item.o_qty);
            }
            series.Font = new Font("Arial", 12.0f, FontStyle.Bold);
            series.ChartType = SeriesChartType.Column;
            series["PieLabelStyle"] = "Outside";
            //series.Color = System.Drawing.Color.Red;
            chart.Series.Add(series);
            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Png;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;
            return new FileStreamResult(returnStream, "image/png");




            //var data = db.GetChart().ToList();
            //var chart = new Chart
            //{
            //    Width = 800,
            //    Height = 450,
            //    RenderType = RenderType.ImageTag,
            //    AntiAliasing = AntiAliasingStyles.All,
            //    TextAntiAliasingQuality = TextAntiAliasingQuality.High
            //};
            //var area = new ChartArea();
            //chart.ChartAreas.Add(area);
            //chart.Titles.Add("Total Product Sale");
            //chart.Titles[0].Font = new Font("Arial", 16f);
            //chart.ChartAreas[0].AxisX.Title = "Order Date";
            //chart.ChartAreas[0].AxisY.Title = "No. of items purchsed";
            //chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 12f);
            //chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 12f);
            //chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 10f);
            //chart.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
            //var series = new Series();
            //foreach(var item in data)
            //{
            //    series.Points.AddXY(item.o_date, item.o_qty);
            //}
            //series.Font = new Font("Arial", 12.0f, FontStyle.Bold);
            //series.ChartType = SeriesChartType.Column;
            //series["PieLabelStyle"] = "Outside";
            //series.Color = System.Drawing.Color.Red;
            //chart.Series.Add(series);
            //var returnStream = new MemoryStream();
            //chart.ImageType = ChartImageType.Png;
            //chart.SaveImage(returnStream);
            //returnStream.Position = 0;
            //return new FileStreamResult(returnStream, "image/png");
        }

        [HttpGet]
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(adminn adm)
        {
            adminn ad = db.adminns.Where(x => x.ad_name == adm.ad_name && x.ad_password == adm.ad_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                Session["ad_name"] = ad.ad_name.ToString();
                return RedirectToAction("Category");
            }
            else
            {
                ViewBag.error = "Invalid Username or Password";
            }
            return View();
        }


        public ActionResult Category()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Category(category cat, HttpPostedFileBase imgfile)
        {
            adminn ad = new adminn();
            string path = uploadimage(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                category ca = new category();
                ca.cat_name = cat.cat_name;
                ca.cat_image = path;
                ca.cat_status = 1;
                ca.ad_id_fk = Convert.ToInt32(Session["ad_id"].ToString());
                db.categories.Add(ca);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View();
        }

        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.categories.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<category> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        //public ActionResult Delete(int? id)
        //{

        //    category c = db.categories.Where(x => x.cat_id == id).SingleOrDefault();
        //    db.categories.Remove(c);
        //    db.SaveChanges();
        //    return RedirectToAction("Category");
        //}

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (RoyalProjectEntities db = new RoyalProjectEntities())
            {
                return View(db.categories.Where(x => x.cat_id == id).SingleOrDefault());
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                using (RoyalProjectEntities db = new RoyalProjectEntities())
                {
                    category c = db.categories.Where(x => x.cat_id == id).SingleOrDefault();
                    db.categories.Remove(c);
                    db.SaveChanges();
                    return RedirectToAction("ViewCategory");
                }

            }
            catch
            {
                return View();
            }
        }

        public ActionResult AdminProducts(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.categories.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<category> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        

        [HttpGet]
        public ActionResult AdminCreateAdd()
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult AdminCreateAdd(product p, HttpPostedFileBase imgfile)
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            adminn ad = new adminn();
            string path = uploadimage(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                product pr = new product();
                pr.pro_name = p.pro_name;
                pr.pro_price = p.pro_price;
                pr.pro_image = path;
                pr.cat_id_fk = p.pro_ad_id_fk;
                //pr.pro_desc = p.pro_desc;
                pr.pro_ad_id_fk = Convert.ToInt32(Session["ad_id"].ToString());
                db.products.Add(pr);
                db.SaveChanges();
                Response.Redirect("AdminProducts");
            }
            return View();
        }

        public ActionResult AdminDisplayAdd(int? id, int? page)
        {
            TempData.Keep();
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            //var list = db.products.Where(x => x.pro_ad_id_fk == id).OrderByDescending(x => x.pro_id).ToList();
            var list = db.products.Where(model => model.cat_id_fk == id).OrderByDescending(model => model.pro_ad_id_fk).ToList();
            IPagedList<product> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        [HttpPost]
        public ActionResult AdminDisplayAdd(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.products.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<product> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }


        public ActionResult AdminViewAdds(int? id)
        {
            ad_view_model adm = new ad_view_model();

            product p = db.products.Where(x => x.pro_id == id).SingleOrDefault();
            adm.pro_id = p.pro_id;
            adm.pro_name = p.pro_name;
            adm.pro_image = p.pro_image;
            adm.pro_price = p.pro_price;
            //adm.pro_desc = p.pro_desc;

            category cat = db.categories.Where(x => x.cat_id == p.cat_id_fk).SingleOrDefault();
            adm.cat_name = cat.cat_name;
            adminn a = db.adminns.Where(x => x.ad_id == p.pro_ad_id_fk).SingleOrDefault();
            adm.ad_name = a.ad_name;
            adm.pro_ad_id_fk = a.ad_id;
            return View(adm);
        }

        public ActionResult Add_Delete(int? id)
        {

            product p = db.products.Where(x => x.pro_id == id).SingleOrDefault();
            db.products.Remove(p);
            db.SaveChanges();
            return RedirectToAction("AdminProducts");
        }

        public ActionResult Admin_Page()
        {
            return View();
        }

        public ActionResult View_Orders()
        {
            return View(db.admin_order_view.ToList());
        }

        public ActionResult View_Contact_Us()
        {
            return View(db.contacts.ToList());
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg, jpeg or png formats are acceptable....');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file');</script>");
                path = "-1";
            }
            return path;
        }
    }
}