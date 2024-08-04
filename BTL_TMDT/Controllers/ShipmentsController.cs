using BTL_TMDT.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL_TMDT.Controllers
{
    public class ShipmentsController : Controller
    {
        private string connectionString = "Data Source=LAPTOP-UIPQ88GB;Initial Catalog=thuongmaidientudb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        // GET: Shipments
        public ActionResult Index()
        {
            return View();
        }

        public List<Shipment> GetShipmentsByUserId(int userId)
        {
            List<Shipment> shipments = new List<Shipment>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        *
                    FROM
                        Shipments
                    WHERE
                        user_id = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    shipments.Add(new Shipment()
                    {
                        shipment_id = (int)reader["shipment_id"],
                        user_id = (int)reader["user_id"],
                        shipment_address = (string)reader["shipment_address"],
                        shipment_city = (string)reader["shipment_city"],
                        shipment_country = (string)reader["shipment_country"],
                        shipment_zip_code = (string)reader["shipment_zip_code"]
                    });
                }
                reader.Close();
            }
            return shipments;
        }
    }
}