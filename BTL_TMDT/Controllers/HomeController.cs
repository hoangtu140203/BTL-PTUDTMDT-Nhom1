using BTL_TMDT.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL_TMDT.Controllers
{
    public class HomeController : Customer_BaseController
    {
        private readonly string connectionString = "Data Source=DESKTOP-OEMIQ40\\SQLEXPRESS;Initial Catalog=thuongmaidientudb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        private ProductController productController = new ProductController();

        public ActionResult Index()
        {
            var bestSellingProducts = GetBestSellingProducts();
            var newProducts = GetNewProducts();

            ViewBag.BestSellingProducts = bestSellingProducts;
            ViewBag.NewProducts = newProducts;

            return View();
        }

        public List<Product> GetBestSellingProducts()
        {
            var bestSellingProducts = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string  query = @"
                    SELECT
                        TOP 10 Products.product_id,
                        SUM(Order_Items.quantity) AS TotalSold
                    FROM
                        Products
                        JOIN Order_Items ON Products.product_id = Order_Items.product_id
                    GROUP BY
                        Products.product_id
                    ORDER BY
                        TotalSold DESC;
                ";


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = productController.GetProductByID((int)reader["product_id"]);
                            bestSellingProducts.Add(product);
                        }
                    }
                }
            }

            return bestSellingProducts;
        }

        private List<Product> GetNewProducts()
        {
            var newProducts = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        TOP 10 Products.product_id
                    FROM
                        Products
                    WHERE
                        is_new = 1
                    ORDER BY
                        product_id DESC
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = productController.GetProductByID((int)reader["product_id"]);
                            newProducts.Add(product);
                        }
                    }
                }
            }

            return newProducts;
        }


    }

}