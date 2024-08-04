using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BTL_TMDT.Models;

namespace BTL_TMDT.Controllers
{
    public class CategoriesController : Customer_BaseController
    {
        // Database connection string
        private string connectionString = "Data Source=DESKTOP-OEMIQ40\\SQLEXPRESS;Initial Catalog=thuongmaidientudb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        // GET: Categories
        public ActionResult Index()
        {
            return View();
        }

        public List<Category> GetCategories()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        category_id,
                        category_name
                    FROM
                        Categories";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Category> categories = new List<Category>();
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        category_id = (int)reader["category_id"],
                        category_name = (string)reader["category_name"]
                    });
                }
                return categories;
            }
        }
    }
}