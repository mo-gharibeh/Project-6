using E_commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_commerce.Controllers
{
    public class UserController : Controller
    {
        public E_commerceEntities db = new E_commerceEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            try
            {
                // TODO: Add insert logic here
                string fullName = form["fullName"];
                string userName = form["username"];
                string password = form["password"];
                string email = form["email"];
                string conPassword = form["confirmPassword"];

                var user = new User
                {
                    Username = userName,
                    Email = email,
                    Password = password,
                    FullName = fullName

                };


                if (conPassword == password) {

                    db.Users.Add(user);
                    db.SaveChanges();

                    var cart = new Cart
                    {
                        UserID = user.UserID // استخدام UserID من المستخدم الجديد
                    };

                    // إضافة السلة الجديدة إلى جدول Carts
                    db.Carts.Add(cart);
                    db.SaveChanges(); // حفظ التغييرات
                    //ModelState.AddModelError("PasswordMismatch", "Succesfull Register");
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                    return View();
                }
               
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["name"] = "1";
            return RedirectToAction("Home", "Categories");
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string username = form["Username"];
            string password = form["Password"];

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("UserNotFound", "User not found.");
                return View();
            }

            if (password == user.Password)
            {
                Session["name"] = "0";
                Session["UserID"] = user.UserID;
                return RedirectToAction("Home", "Categories");
            }
            else
            {
                ModelState.AddModelError("PasswordMismatch", "Invalid username or password.");
                return View();
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
