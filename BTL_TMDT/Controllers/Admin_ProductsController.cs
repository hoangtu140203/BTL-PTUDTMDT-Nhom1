using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTL_TMDT.Models;

namespace BTL_TMDT.Controllers
{
    public class Admin_ProductsController : Controller
    {
        private Db_TMDT db = new Db_TMDT();

        // GET: Products
        public ActionResult Index()
        {
            if ((string)Session["role"] != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if ((string)Session["role"] != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if ((string)Session["role"] != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,category_id,product_name,description,price,discount_price,stock,brand,is_new")] Product product)
        {
            if (ModelState.IsValid)
            {
                if ((string)Session["role"] != "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                var f1 = Request.Files["FileName1"];
                var f2 = Request.Files["FileName2"];
                var f3 = Request.Files["FileName3"];
                if (f1 != null && f1.ContentLength > 0)
                {
                    string tenfile1 = System.IO.Path.GetFileName(f1.FileName);
                    string duongdan1 = Server.MapPath("~/Images/" + tenfile1);
                    f1.SaveAs(duongdan1);
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = tenfile1;
                    db.Product_Images.Add(pi);
                }
                else
                {
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = "noimage.png";
                    db.Product_Images.Add(pi);
                }
                if (f2 != null && f2.ContentLength > 0)
                {
                    string tenfile2 = System.IO.Path.GetFileName(f2.FileName);
                    string duongdan2 = Server.MapPath("~/Images/" + tenfile2);
                    f2.SaveAs(duongdan2);
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = tenfile2;
                    db.Product_Images.Add(pi);
                }
                else
                {
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = "noimage.png";
                    db.Product_Images.Add(pi);
                }
                if (f3 != null && f3.ContentLength > 0)
                {
                    string tenfile3 = System.IO.Path.GetFileName(f3.FileName);
                    string duongdan3 = Server.MapPath("~/Images/" + tenfile3);
                    f3.SaveAs(duongdan3);
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = tenfile3;
                    db.Product_Images.Add(pi);
                }
                else
                {
                    Product_Images pi = new Product_Images();
                    pi.product_id = product.product_id;
                    pi.image_url = "noimage.png";
                    db.Product_Images.Add(pi);
                }
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "product_id,category_id,product_name,description,price,discount_price,stock,brand,is_new")] Product product)
        {
            if (ModelState.IsValid)
            {
                var f1 = Request.Files["FileName1"];
                var f2 = Request.Files["FileName2"];
                var f3 = Request.Files["FileName3"];
                if (f1 != null && f1.ContentLength > 0)
                {
                    string tenfile1 = System.IO.Path.GetFileName(f1.FileName);
                    string duongdan1 = Server.MapPath("~/Images/" + tenfile1);
                    f1.SaveAs(duongdan1);
                    Product_Images pi = db.Product_Images.Where(p => p.product_id == product.product_id).ToList()[0];
                    pi.image_url = tenfile1;
                    db.Product_Images.AddOrUpdate(pi);
                }
                if (f2 != null && f2.ContentLength > 0)
                {
                    string tenfile2 = System.IO.Path.GetFileName(f2.FileName);
                    string duongdan2 = Server.MapPath("~/Images/" + tenfile2);
                    f2.SaveAs(duongdan2);
                    Product_Images pi = db.Product_Images.Where(p => p.product_id == product.product_id).ToList()[1];
                    pi.image_url = tenfile2;
                    db.Product_Images.AddOrUpdate(pi);
                }
                if (f3 != null && f3.ContentLength > 0)
                {
                    string tenfile3 = System.IO.Path.GetFileName(f3.FileName);
                    string duongdan3 = Server.MapPath("~/Images/" + tenfile3);
                    f3.SaveAs(duongdan3);
                    Product_Images pi = db.Product_Images.Where(p => p.product_id == product.product_id).ToList()[2];
                    pi.image_url = tenfile3;
                    db.Product_Images.AddOrUpdate(pi);
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category_id = new SelectList(db.Categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        DataUtil data = new DataUtil();
        public ActionResult SellingProduct()
        {
            var selling = data.GetSellingProduct();
            return View(selling);
        }
        public ActionResult UnsoldProduct()
        {
            var Unsold = data.GetUnsoldProduct();
            return View(Unsold);
        }
    }
}
