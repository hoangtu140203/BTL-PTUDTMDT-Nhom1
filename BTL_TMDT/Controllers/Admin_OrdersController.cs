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
    public class Admin_OrdersController : Controller
    {
        private Db_TMDT db = new Db_TMDT();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Shipment).Include(o => o.User);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.shipment_id = new SelectList(db.Shipments, "shipment_id", "shipment_address");
            ViewBag.user_id = new SelectList(db.Users, "user_id", "first_name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_id,shipment_id,user_id,order_date,total_amount,status,payment_method,order_note")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.shipment_id = new SelectList(db.Shipments, "shipment_id", "shipment_address", order.shipment_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "first_name", order.user_id);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.shipment_id = new SelectList(db.Shipments, "shipment_id", "shipment_address", order.shipment_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "first_name", order.user_id);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,shipment_id,user_id,order_date,total_amount,status,payment_method,order_note")] Order order)
        {
            if (ModelState.IsValid)
            {
                //[Bind(Include = "order_id,shipment_id,user_id,order_date,total_amount,status,payment_method,order_note")]
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.shipment_id = new SelectList(db.Shipments, "shipment_id", "shipment_address", order.shipment_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "first_name", order.user_id);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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

        DataUtil DU =new DataUtil();

        // Action to get orders data (you can replace this with actual data fetching logic)
        public JsonResult GetOrdersData(string timeRange)
        {
            var groupedData = GroupOrdersData(timeRange);
            return Json(groupedData, JsonRequestBehavior.AllowGet);
        }


        private object GroupOrdersData(string timeRange)
        {

            if (timeRange == "day")
            {
                var data = new DataContainer
                {
                    date = DU.GetDateTimeDay(),
                    count = DU.GetCountDay(),
                    revenue = DU.GetRevenueDay(),
                };
                return data;
            }
            else if (timeRange == "month")
            {
                var data = new DataContainer
                {
                    date = DU.GetDateTimeMonth(),
                    count = DU.GetCountMonth(),
                    revenue = DU.GetRevenueMonth(),
                };
                return data;
            }
            else if (timeRange == "year")
            {
                var data = new DataContainer
                {
                    date = DU.GetDateTimeYear(),
                    count = DU.GetCountYear(),
                    revenue = DU.GetRevenueYear(),
                };
                Console.WriteLine(data);
                return data;
            }

            return null;
        }

        public ActionResult Statistical()
        {
            return View();
        }
    }
}
