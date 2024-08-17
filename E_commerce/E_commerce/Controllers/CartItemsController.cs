using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_commerce.Models;

namespace E_commerce.Controllers
{
    public class CartItemsController : Controller
    {
        private E_commerceEntities db = new E_commerceEntities();

        // GET: CartItems
        public ActionResult Index()
        {
            var cartItems = db.CartItems.Include(c => c.Cart).Include(c => c.Product);
            return View(cartItems.ToList());
        }

        public ActionResult Cart(int? id)
        {
            // افترض أنك تحصل على UserID من الجلسة أو من مصدر آخر
            //int userId = ...;  وضع الكود المناسب للحصول على UserID للمستخدم الحالي

            // الحصول على السلة والعناصر المرتبطة بها
            var cartItems = db.Carts
                              .Where(c => c.UserID == id)
                              .SelectMany(c => c.CartItems)
                              .Include(ci => ci.Product) // جلب تفاصيل المنتج
                              .ToList();

            return View(cartItems);
        }

        public ActionResult Create(int id , int userID)
        {

            // افترض أنك تحصل على UserID من الجلسة أو من مصدر آخر
            int userId = userID; // وضع الكود المناسب للحصول على UserID للمستخدم الحالي

            // تحقق مما إذا كان المستخدم يمتلك سلة بالفعل
            var cart = db.Carts.SingleOrDefault(c => c.UserID == userId);

            // إذا لم يكن لديه سلة، أنشئ واحدة جديدة
            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userId
                };
                db.Carts.Add(cart);
                db.SaveChanges(); // حفظ السلة الجديدة
            }

            // تحقق مما إذا كان العنصر موجود بالفعل في السلة
            var existingCartItem = db.CartItems
                                     .SingleOrDefault(ci => ci.CartID == cart.CartID && ci.ProductID == id);

            if (existingCartItem != null)
            {
                // إذا كان العنصر موجوداً بالفعل، قم بزيادة الكمية
                existingCartItem.Quantity++;
            }
            else
            {
                // إذا كان العنصر غير موجود في السلة، قم بإضافته
                var cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = id,
                    Quantity = 1 // تبدأ الكمية من 1
                };
                db.CartItems.Add(cartItem);
            }

            db.SaveChanges(); // حفظ التغييرات

            // إعادة التوجيه إلى صفحة السلة أو البقاء في نفس الصفحة
            return RedirectToAction("Shop", "Products");
        }

        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartItem = db.CartItems.Find(id);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                db.SaveChanges();
            }
            else if (cartItem != null && quantity == 0)
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
            }

            return RedirectToAction("Cart", new { id = Session["UserID"] /*cartItem.Cart.UserID*/ });
        }

        public ActionResult Remove(int id)
        {
            var cartItem = db.CartItems.Find(id);

            if (cartItem != null)
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
            }

            return RedirectToAction("Cart", new { id = Session["UserID"] /*cartItem.Cart.UserID*/ });
        }


     /// <summary>
     /// ///////////////////////////
     /// </summary>
     /// <param name="id"></param>
     /// <returns></returns>

        // GET: CartItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // GET: CartItems/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID");
        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
        //    return View();
        //}

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "CartItemID,CartID,ProductID,Quantity")] CartItem cartItem)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        db.CartItems.Add(cartItem);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cartItem.CartID);
        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cartItem.ProductID);
        //    return View(cartItem);
        //}




        // GET: CartItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cartItem.CartID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cartItem.ProductID);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartItemID,CartID,ProductID,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cartItem.CartID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cartItem.ProductID);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CartItem cartItem = db.CartItems.Find(id);
            db.CartItems.Remove(cartItem);
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
