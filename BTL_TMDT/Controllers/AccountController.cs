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
    public class AccountController : Customer_BaseController
    {
        // Database connection string
        private string connectionString = "Data Source=DESKTOP-OEMIQ40\\SQLEXPRESS;Initial Catalog=thuongmaidientudb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";


        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(string firstName, string lastName, string email, string password, string phone, string confirm_password)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string checkQuery = @"
                    SELECT
                        COUNT(*)
                    FROM
                        Users
                    WHERE
                        email = @Email
                        OR phone = @Phone";

                string insertQuery = @"
                    INSERT INTO
                        Users (first_name, last_name, email, password, phone, role)
                    VALUES
                        (@FirstName, @LastName, @Email, @Password, @Phone, 'customer')";

                try
                {
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    checkCmd.Parameters.AddWithValue("@Phone", phone);

                    con.Open();

                    int existingUserCount = (int)checkCmd.ExecuteScalar();
                    if (existingUserCount > 0)
                    {
                        ViewBag.MessageRegister = "Email hoặc số điện thoại đã tồn tại. Vui lòng thử lại.";
                        return View(); 
                    }

                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Phone", phone);

                    cmd.ExecuteNonQuery();

                    ViewBag.MessageLogin = "Đăng ký tài khoản thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.MessageRegister = "Thông tin đăng ký không hợp lệ: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return RedirectToAction("Login");
        }


        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        *
                    FROM
                        Users
                    WHERE
                        email = @Email
                        AND password = @Password";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Session["UserId"] = reader["user_id"];
                    Session["UserName"] = reader["first_name"] + " " + reader["last_name"];
                    Session["Role"] = reader["role"];
                    if ((string)reader["role"] == "admin")
                    {
                        return RedirectToAction("Index", "Admin_Products");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.MessageLogin = "Thông tin đăng nhập không hợp lệ";
                }
            }
            return View();
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }


        public User GetUserById(int userId)
        {
            User user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        *
                    FROM
                        Users
                    WHERE
                        user_id = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User()
                    {
                        first_name = (string)reader["first_name"],
                        last_name = (string)reader["last_name"],
                        email = (string)reader["email"],
                        phone = (string)reader["phone"]
                    };
                }
                reader.Close();
            }
            return user;
        }


        public ActionResult ShipmentManagement()
        {
            if ((string)Session["role"] != "customer")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.username = (string)@Session["UserName"];
            ViewBag.Shipments = new ShipmentsController().GetShipmentsByUserId((int)Session["UserId"]);
            return View();
        }

    }
}