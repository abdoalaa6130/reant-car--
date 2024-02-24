using RentCars.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCars.Controllers
{
    public class adminController : Controller
    {
        RentCarEntities db = new RentCarEntities();
        SqlConnection con = new SqlConnection(@"data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\RentCar.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;");

        // GET: admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult approve()
        {
            return View();
        }
        public ActionResult ProductList()
        {
            return View(db.cars.OrderByDescending(x => x.Id).ToList());
        }
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            string extension = Path.GetExtension(file.FileName);


            path = Path.Combine(Server.MapPath("~/img"), random + Path.GetFileName(file.FileName));
            file.SaveAs(path);
            path = "~/img/" + random + Path.GetFileName(file.FileName);

            //    ViewBag.Message = "File uploaded successfully";




            return path;
        }
        public ActionResult login(string mess="")
        {
            User m = new User();
            m.mess = mess;
            return View(m);
        }
  
        [HttpPost]
        public ActionResult login(User user)
        {
            User v = db.User.Where(x => x.Email == user.Email && x.Password == user.Password &&x.type== "employee").SingleOrDefault();

            if (v != null)
            {

                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Login", new { mess = "يوجد خطا فى البريد الالكترونى او كلمة المرور" });


            }
        }
        public ActionResult adduser(string mess="")
        {
            User m = new User();
            m.mess = mess;
            return View(m);
        }
        [HttpPost]

        public ActionResult adduser(User user )
        {
            if (db.User.Where(x => x.Email == user.Email).SingleOrDefault() != null) {
                return RedirectToAction("adduser", new { mess = "هذا الايميل موجود بالفعل" });

            }
            else
            {
                User m = new User();

                m.UserName = user.UserName;
                m.Phone = user.Phone;
                m.Password = user.Password;
                m.type = "employee";
                m.Email = user.Email;
                db.User.Add(m);
                db.SaveChanges();

                return RedirectToAction("userList");

            }




        }
        public ActionResult userList()
        {
            return View(db.User.Where(x=>x.type=="employee").ToList());
        }

        public ActionResult Products(int? id)
        {
            cars ad = new cars();

            if (id != null)
            {
                cars p = db.cars.Where(x => x.Id == id).SingleOrDefault();
                ad.Mark = p.Mark;
                ad.Model = p.Model;
                ad.cat = p.cat;
                ad.No = p.No;
                ad.adaptation = p.adaptation;
                ad.NoOfDoors = p.NoOfDoors;
                p.Id = p.Id;
                ad.Image = p.Image;
                ad.priceForDay = p.priceForDay;
                return View(ad);

            }
            else
            {
                return View();

            }
        }

        [HttpPost]
        public ActionResult Products(cars cars, HttpPostedFileBase imgfile)
        {
            if (cars.Id != 0)
            {
                using (RentCarEntities dp = new RentCarEntities())
                {
                    db.Entry(cars).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("productList");

            }
            else
            {
                string path = uploadimgfile(imgfile);
                cars p = new cars();
                p.Mark = cars.Mark;
                p.Model = cars.Model;
                p.cat = cars.cat;
                p.No = cars.No;
                p.adaptation = cars.adaptation;
                p.Image = path;
                p.NoOfDoors = cars.NoOfDoors;
                p.priceForDay = cars.priceForDay;

                db.cars.Add(p);
                db.SaveChanges();
                return RedirectToAction("productList");
            }
        }
        public ActionResult Delete(int id, FormCollection collection)
        {
            using (RentCarEntities dbmodel = new RentCarEntities())
            {
                cars m = dbmodel.cars.Where(x => x.Id == id).FirstOrDefault();
                dbmodel.cars.Remove(m);
                dbmodel.SaveChanges();

            }

            return RedirectToAction("productList");
        }
        public ActionResult userorders()
        {
            UserOrderModel ad = new UserOrderModel();

            ad.rentRequsts = (
                                from e in db.cars
                                from s in db.RentRequst.Where(c => c.CarId == e.Id.ToString()&&c.Pay=="true")
                                select new RentRequsetModel
                                {
                                    Id = s.Id,
                                    CustomerName = s.CustomerName,
                                    Mark = e.Mark,
                                    Model = e.Model,
                                    Image = e.Image,
                                    CustomerPhone = s.CustomerPhone,
                                    priceForDay = e.priceForDay,
                                    RentDuration = s.Rentduration,
                                    total = s.Rentduration * e.priceForDay,
                                    deliverydate = s.deliverydate



                                }).OrderByDescending(x => x.Id).ToList();


            return View(ad);

        }
       
        public ActionResult Deleteuserorder(int id, FormCollection collection)
        {
            using (RentCarEntities dbmodel = new RentCarEntities())
            {
                RentRequst m = dbmodel.RentRequst.Where(x => x.Id == id).FirstOrDefault();
                dbmodel.RentRequst.Remove(m);
                dbmodel.SaveChanges();
                SqlCommand n;

                n = new SqlCommand("update cars set [book]=@b where Id=@id", con);
                n.Parameters.AddWithValue("@b", "");
                n.Parameters.AddWithValue("@id", m.CarId);

                con.Open();
                n.ExecuteNonQuery();

                con.Close();
            }

            return RedirectToAction("userorders");


        }

    }
}