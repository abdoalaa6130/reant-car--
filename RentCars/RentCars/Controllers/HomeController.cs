using RentCars.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCars.Controllers
{
    public class HomeController : Controller
    {
        RentCarEntities db = new RentCarEntities();
        SqlConnection con = new SqlConnection(@"data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\RentCar.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;");


        public ActionResult Index(string email = "")
        {
            cars ad = new cars();
            var usr = db.User.Where(x => x.Email == email).SingleOrDefault();
            ad.email = usr.Email;

            return View(ad);
        }
        [HttpPost]
        public ActionResult Index(cars cars)
        {

            return RedirectToAction("Products", new { Model = cars.Model, no = cars.No, adat = cars.adaptation, cat = cars.cat,email=cars.email });
        }
        public ActionResult Products(string Model, int no, string adat, string cat,string email)
        {
            usercarmodel ad = new usercarmodel();
            ad.Email = email;
            ad.carmodels = db.cars.Where(x => x.Model == Model&&x.No==no&&x.adaptation==adat&&x.cat== cat && x.book != "book").Select(s => new carmodel
            {
                Id = s.Id,
              cat=s.cat,
              adaptation=s.adaptation,
              Image=s.Image,
              Model=s.Model,
              No=s.No,
              Mark=s.Mark,
              priceForDay=s.priceForDay,
              NoOfDoors=s.NoOfDoors
            }).OrderByDescending(x => x.Id).ToList();
            return View(ad);
        }
        public ActionResult singleProduct(int idcar,string email,string mess="")
        {
            RentvarMode n = new RentvarMode();
            var b = db.cars.Where(x => x.Id == idcar).SingleOrDefault();
            n.Model = b.Model;
            n.cat = b.cat;
            n.Mark = b.Mark;
            n.mess = mess;
            n.adaptation = b.adaptation;
            n.No = b.No;
            n.NoOfDoors = b.NoOfDoors;
            n.Image = b.Image;
            n.priceForDay = b.priceForDay;
            n.Id = idcar;
            n.email = email;
            return View(n);
        }
        [HttpPost]
        public ActionResult singleProduct(RentvarMode r)
        {
            RentRequst b = new RentRequst();
            b.CarId = r.Id.ToString();
            b.CustomerName = r.CustomerName;
            b.CustomerPhone = r.CustomerPhone;
            b.Rentduration = r.RentDuration;
            b.deliverydate = r.deliverydate;
            b.Occasion = r.Occasion; ;
            b.Additions = r.Additions;
            b.email = r.email;

            var t = db.cars.Where(x => x.Id == r.Id).SingleOrDefault();
            b.total = r.RentDuration * t.priceForDay;
            db.RentRequst.Add(b);
            db.SaveChanges();
            SqlCommand n;

            n = new SqlCommand("update cars set [book]=@b where Id=@id", con);
            n.Parameters.AddWithValue("@b", "book");
            n.Parameters.AddWithValue("@id", r.Id);

            con.Open();
            n.ExecuteNonQuery();

            con.Close();
            return RedirectToAction("singleProduct", new { email =r.email, idcar =r.Id,mess="تم حجز السيارة بنجاح"});
        }
        public ActionResult yourCard(string email)
        {
            UserOrderModel ad = new UserOrderModel();
            ad.email = email;
            ad.rentRequsts = (
                                           from e in db.cars
                                           from s in db.RentRequst.Where(c => c.CarId == e.Id.ToString()&&c.email==email)
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
        public ActionResult cash(string email)
        {
            SqlCommand n;

            n = new SqlCommand("update RentRequst set [Pay]=@b", con);
            n.Parameters.AddWithValue("@b", "true");
            con.Open();
            n.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Index",new { email = email });
        }
        public ActionResult visa(string email)
        {
            SqlCommand n;

            n = new SqlCommand("update RentRequst set [Pay]=@b", con);
            n.Parameters.AddWithValue("@b", "true");
            con.Open();
            n.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Index", new { email = email });
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

            return RedirectToAction("yourCard");


        }
        public ActionResult AllProducts(string email)
        {

            usercarmodel ad = new usercarmodel();
            ad.email = email;
            ad.carmodels = db.cars.Where(x => x.book != "book").Select(s => new carmodel
            {
                Id = s.Id,
                cat = s.cat,
                adaptation = s.adaptation,
                Image = s.Image,
                Model = s.Model,
                No = s.No,
                Mark = s.Mark,
                priceForDay = s.priceForDay,
                NoOfDoors = s.NoOfDoors
            }).OrderByDescending(x => x.Id).ToList();
            return View(ad);
        }
        public ActionResult login(string mess = "")
        {
            User m = new User();
            m.mess = mess;
            return View(m);
        }

        [HttpPost]
        public ActionResult login(User user)
        {
            User v = db.User.Where(x => x.Email == user.Email && x.Password == user.Password&&x.type=="user").SingleOrDefault();

            if (v != null)
            {

                return RedirectToAction("Index", new { email = user.Email });

            }
            else
            {
                return RedirectToAction("Login", new { mess = "يوجد خطا فى البريد الالكترونى او كلمة المرور" });


            }
        }
        public ActionResult adduser(string mess = "")
        {
            User m = new User();
            m.mess = mess;
            return View(m);
        }
        [HttpPost]

        public ActionResult adduser(User user)
        {
            if (db.User.Where(x => x.Email == user.Email).SingleOrDefault() != null)
            {
                return RedirectToAction("adduser", new { mess = "هذا الايميل موجود بالفعل" });

            }
            else
            {
                User m = new User();

                m.UserName = user.UserName;
                m.Phone = user.Phone;
                m.Password = user.Password;
                m.type = "user";
                m.Email = user.Email;
                db.User.Add(m);
                db.SaveChanges();

                return RedirectToAction("login");

            }




        }
    }
}