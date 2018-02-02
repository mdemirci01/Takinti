using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Takinti.Models;

namespace Takinti.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Cart()
        {
            if (Request.IsAjaxRequest())
            {
                return View("LayoutCart");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Cart(FormCollection form)
        {
            if (Request.IsAjaxRequest())
            {
                return View("LayoutCart");
            }
            if (Session["Cart"] == null)
            {
                Session["Cart"] = new Cart();
            }
            var cartItems = ((Cart)Session["Cart"]).CartItems.ToArray();
            foreach (var cartItem in cartItems)
            {
                if (!String.IsNullOrEmpty(form["Quantity_" + cartItem.Product.Slug.ToLower()]))
                {
                    var sessionCartItem = ((Cart)Session["Cart"]).CartItems
                        .FirstOrDefault(c => c.Product.Slug.ToLower() == cartItem.Product.Slug.ToLower());

                    sessionCartItem.Quantity = Convert.ToInt32(form["Quantity_" + cartItem.Product.Slug.ToLower()]);

                    if (sessionCartItem.Quantity <= 0)
                    {
                        ((Cart)Session["Cart"]).CartItems.Remove(sessionCartItem);
                    }
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult Checkout()
        {
            if (Session["Cart"] == null || ((Cart)Session["Cart"]).ProductCount == 0)
            {
                return RedirectToAction("Cart", new { error = "Sepet boş!" });
            }
            using (var db = new ApplicationDbContext()) {
                
                ViewBag.Countries = new SelectList(db.Countries.OrderBy(c => c.Name).ToList(), "Id", "Name");
                ViewBag.Cities = new SelectList(db.Cities.OrderBy(c => c.Name).ToList(), "Id", "Name");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel checkout)
        {
            if (ModelState.IsValid) { 
            if (Session["Cart"] == null || ((Cart)Session["Cart"]).ProductCount == 0)
            {
                return RedirectToAction("Cart", new { error = "Sepet boş!" });
            }
            using (var db = new ApplicationDbContext())
            {
                // sepeti veritabanına kaydet
                var cart = (Cart)Session["Cart"];
                cart.UserName = User.Identity.Name;
                db.Carts.Add(cart);
                db.SaveChanges();

                // siparişi veritabanına kaydet
                var order = new Order();
                order.CartId = cart.Id;
                order.FullName = checkout.FullName;
                order.IdentityNumber = checkout.IdentityNumber;
                order.Phone = checkout.Phone;
                order.PostalCode = checkout.PostalCode;
                order.CountryId = checkout.CountryId;
                order.CityId = checkout.CityId;
                order.Address = checkout.Address;
                order.CardHolderName = checkout.CardHolderName;
                order.CardNumberLastFourDigit = checkout.CardNumber.Substring(12, 4);
                order.CompanyName = checkout.CompanyName;
                order.Email = checkout.Email;
                order.UserName = User.Identity.Name;
                order.OrderStatus = OrderStatus.OrderReviewing;
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem();
                    orderItem.ProductId = cartItem.ProductId;
                    var product = db.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                    orderItem.Product = product;
                    orderItem.ProductName = product.Name;
                    orderItem.Quantity = cartItem.Quantity;
                    orderItem.ProductPrice = product.Price;
                    orderItem.ProductTax = product.Tax;
                    order.OrderItems.Add(orderItem);
                }
                // ödeme işlemi yapılır
                /*
                 * var result = PaymentGateway.GetPayment(cart.TotalPrice);
                 * if (result == success) öde yapılmıştır hata varsa hata kullanıcıya gösterilir
                 */
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("ThankYou");
            }
            }
            using (var db = new ApplicationDbContext()) { 
            ViewBag.Countries = new SelectList(db.Countries.OrderBy(c => c.Name).ToList(), "Id", "Name", checkout.CountryId);
            ViewBag.Cities = new SelectList(db.Cities.OrderBy(c => c.Name).ToList(), "Id", "Name", checkout.CityId);
            }
            return View(checkout);
        }
        public JsonResult AddToCart(string slug)
        {
            using (var db = new ApplicationDbContext())
            {
                if (Session["Cart"] == null)
                {
                    Session["Cart"] = new Cart();
                    ((Cart)Session["Cart"]).CreateDate = DateTime.Now;
                }

                ((Cart)Session["Cart"]).UserName = User.Identity.Name;
                ((Cart)Session["Cart"]).UpdateDate = DateTime.Now;

                CartItem cartItem = ((Cart)Session["Cart"]).CartItems
                    .FirstOrDefault(c => c.Product.Slug.ToLower() == slug.ToLower());
                if (cartItem == null)
                {
                    cartItem = new CartItem();
                    cartItem.Quantity = 1;
                    var product = db.Products.FirstOrDefault(p =>
                    p.Slug.ToLower() == slug.ToLower()
                    && p.IsInStock == true && p.Quantity > 0 && p.IsPublished == true);
                    if (product == null)
                    {
                        return Json(false);
                    }
                    cartItem.ProductId = product.Id;
                    cartItem.Product = product;
                    cartItem.CreateDate = DateTime.Now;
                    ((Cart)Session["Cart"]).CartItems.Add(cartItem);
                } else {
                    cartItem.Quantity += 1;
                }
                
                return Json(CartProductCount());
            }
        }
        public JsonResult RemoveFromCart(string slug)
        {
            if (Session["Cart"] == null)
            {
                Session["Cart"] = new Cart();
            }

            var cartItem = ((Cart)Session["Cart"]).CartItems
                .FirstOrDefault(c => c.Product.Slug.ToLower() == slug.ToLower());
            if (cartItem != null)
            {
                ((Cart)Session["Cart"]).CartItems.Remove(cartItem);
            }
            return Json(CartProductCount());
        }
        public int CartProductCount()
        {
            if (Session["Cart"] != null)
            {
                return ((Cart)Session["Cart"]).ProductCount;
            }
            return 0;
        }
        public ActionResult ThankYou()
        {
            return View();
        }
    }
}