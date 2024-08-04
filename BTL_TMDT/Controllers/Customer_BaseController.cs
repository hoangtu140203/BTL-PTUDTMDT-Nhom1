using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BTL_TMDT.Models;

namespace BTL_TMDT.Controllers
{
    public class Customer_BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.Carts = GetCarts();
            ViewBag.Categories = GetCategories();
            ViewBag.Username = GetUsername();

            ViewBag.Query = "";
        }

        private Cart GetCarts()
        {
            return null;
        }

        private List<Category> GetCategories()
        {
            return new CategoriesController().GetCategories();
        }

        private string GetUsername()
        {
            return (string)Session["UserName"];
        }
    }
}