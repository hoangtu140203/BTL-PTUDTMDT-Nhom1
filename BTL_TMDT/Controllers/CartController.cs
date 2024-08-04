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

    public class CartController : Customer_BaseController
    {
        private string connectionString = "Data Source=LAPTOP-UIPQ88GB;Initial Catalog=thuongmaidientudb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        // GET: Cart
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];

            List<Cart> carts = GetCartItemsByUserId(userId);
            return View(carts);
        }

        // POST: Add to Cart
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                if (!IsStockAvailable(con, productId, quantity))
                {
                    TempData["Error"] = "Vượt quá số lượng trong kho.";
                    return RedirectToAction("Index");
                }

                int? currentCartQuantity = GetCurrentCartQuantity(con, userId, productId);

                if (currentCartQuantity.HasValue)
                {
                    int newQuantity = currentCartQuantity.Value + quantity;
                    if (newQuantity > GetProductStock(con, productId))
                    {
                        TempData["Error"] = "Vượt quá số lượng trong kho.";
                        return RedirectToAction("Index");
                    }

                    UpdateCart(con, userId, productId, newQuantity);
                }
                else
                {
                    InsertIntoCart(con, userId, productId, quantity);
                }
            }

            return RedirectToAction("Index");
        }

        private bool IsStockAvailable(SqlConnection con, int productId, int requestedQuantity)
        {
            int currentStock = GetProductStock(con, productId);
            return currentStock >= requestedQuantity;
        }

        private int GetProductStock(SqlConnection con, int productId)
        {
            string query = @"
                SELECT
                    stock
                FROM
                    Products
                WHERE
                    product_id = @ProductId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                return (int)cmd.ExecuteScalar();
            }
        }

        private int? GetCurrentCartQuantity(SqlConnection con, int userId, int productId)
        {
            string query = @"
                SELECT
                    quantity
                FROM
                    Cart
                WHERE
                    user_id = @UserId
                    AND product_id = @ProductId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                object result = cmd.ExecuteScalar();
                return result != null ? (int?)result : null;
            }
        }

        private void UpdateCart(SqlConnection con, int userId, int productId, int quantity)
        {
            string query = @"
                UPDATE
                    Cart
                SET
                    quantity = @Quantity
                WHERE
                    user_id = @UserId
                    AND product_id = @ProductId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertIntoCart(SqlConnection con, int userId, int productId, int quantity)
        {
            string query = @"
                INSERT INTO
                    Cart (user_id, product_id, quantity)
                VALUES
                    (@UserId, @ProductId, @Quantity)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                cmd.ExecuteNonQuery();
            }
        }


        // POST: Remove from Cart
        [HttpPost]
        public ActionResult Remove(int cartId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
                    DELETE FROM
                        Cart
                    WHERE
                        cart_id = @CartId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CartId", cartId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }



        public List<Cart> GetCartItemsByUserId(int userId)
        {
            List<Cart> carts = new List<Cart>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        *
                    FROM
                        Cart c
                        JOIN Products p ON c.product_id = p.product_id
                    WHERE
                        c.user_id = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    carts.Add(new Cart()
                    {
                        cart_id = (int)reader["cart_id"],
                        user_id = (int)reader["user_id"],
                        product_id = (int)reader["product_id"],
                        quantity = (int)reader["quantity"],
                        Product = new ProductController().GetProductByID((int)reader["product_id"])
                    });
                }
                reader.Close();
            }
            return carts;
        }


        public ActionResult Purchase()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            User user = new AccountController().GetUserById(userId);

            if (user != null)
            {
                user.Shipments = new ShipmentsController().GetShipmentsByUserId(userId);
                user.Carts = GetCartItemsByUserId(userId);
            }

            return View(user);
        }


        // POST: Purchase
        [HttpPost]
        public ActionResult Purchase(string recipient_first_name, string recipient_last_name, string recipient_phone, string address, string city, string country, string zipCode, string paymentMethod)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userId = (int)Session["UserId"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryShipment = @"
                    INSERT INTO
                        Shipments (
                            user_id,
                            recipient_first_name,
                            recipient_last_name,
                            recipient_phone,
                            shipment_address,
                            shipment_city,
                            shipment_country,
                            shipment_zip_code
                        ) OUTPUT INSERTED.shipment_id
                    VALUES
                        (@UserId, @RecipientFirstName, @RecipientLastName, @RecipientPhone, @Address, @City, @Country, @ZipCode)";

                SqlCommand cmdShipment = new SqlCommand(queryShipment, con);
                cmdShipment.Parameters.AddWithValue("@UserId", userId);
                cmdShipment.Parameters.AddWithValue("@RecipientFirstName", recipient_first_name);
                cmdShipment.Parameters.AddWithValue("@RecipientLastName", recipient_last_name);
                cmdShipment.Parameters.AddWithValue("@RecipientPhone", recipient_phone);
                cmdShipment.Parameters.AddWithValue("@Address", address);
                cmdShipment.Parameters.AddWithValue("@City", city);
                cmdShipment.Parameters.AddWithValue("@Country", country);
                cmdShipment.Parameters.AddWithValue("@ZipCode", zipCode);
                con.Open();
                int shipmentId = (int)cmdShipment.ExecuteScalar();

                string queryOrder = @"
                    INSERT INTO
                        Orders (
                            shipment_id,
                            user_id,
                            order_date,
                            total_amount,
                            status,
                            payment_method
                        ) OUTPUT INSERTED.order_id
                    VALUES
                        (
                            @ShipmentId,
                            @UserId,
                            @OrderDate,
                            @TotalAmount,
                            'Processing',
                            @PaymentMethod
                        )";

                SqlCommand cmdOrder = new SqlCommand(queryOrder, con);
                cmdOrder.Parameters.AddWithValue("@ShipmentId", shipmentId);
                cmdOrder.Parameters.AddWithValue("@UserId", userId);
                cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmdOrder.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(CalculateTotalAmount(userId, con)));
                cmdOrder.Parameters.AddWithValue("@PaymentMethod", "Thanh toán khi nhận hàng");
                int orderId = (int)cmdOrder.ExecuteScalar();

                string queryOrderItems = @"
                    INSERT INTO
                        Order_Items (order_id, product_id, quantity, price)
                    SELECT
                        @OrderId,
                        c.product_id,
                        c.quantity,
                        p.price
                    FROM
                        Cart c
                        INNER JOIN Products p ON c.product_id = p.product_id
                    WHERE
                        c.user_id = @UserId";

                SqlCommand cmdOrderItems = new SqlCommand(queryOrderItems, con);
                cmdOrderItems.Parameters.AddWithValue("@OrderId", orderId);
                cmdOrderItems.Parameters.AddWithValue("@UserId", userId);
                cmdOrderItems.ExecuteNonQuery();

                // Update stock quantity for each product purchased
                string queryUpdateStock = @"
                    UPDATE
                        Products
                    SET
                        stock = stock - c.quantity
                    FROM
                        Products p
                        INNER JOIN Cart c ON p.product_id = c.product_id
                    WHERE
                        c.user_id = @UserId";

                SqlCommand cmdUpdateStock = new SqlCommand(queryUpdateStock, con);
                cmdUpdateStock.Parameters.AddWithValue("@UserId", userId);
                cmdUpdateStock.ExecuteNonQuery();

                string queryClearCart = @"
                    DELETE FROM
                        Cart
                    WHERE
                        user_id = @UserId";

                SqlCommand cmdClearCart = new SqlCommand(queryClearCart, con);
                cmdClearCart.Parameters.AddWithValue("@UserId", userId);
                cmdClearCart.ExecuteNonQuery();
            }
            return RedirectToAction("OrderHistory");
        }


        // GET: Order History
        public ActionResult OrderHistory()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userId = (int)Session["UserId"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        o.order_id,
                        o.order_date,
                        o.total_amount,
                        o.status,
                        s.shipment_address,
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                Order_Items oi
                            WHERE
                                oi.order_id = o.order_id
                        ) AS NumberOfProducts
                    FROM
                        Orders o
                        JOIN Shipments s ON o.shipment_id = s.shipment_id
                    WHERE
                        o.user_id = @UserId
                    ORDER BY
                        o.order_date DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Order> orders = new List<Order>();
                Dictionary<int, string> shipmentAddresses = new Dictionary<int, string>();
                Dictionary<int, int> productCounts = new Dictionary<int, int>();
                while (reader.Read())
                {
                    int orderId = (int)reader["order_id"];
                    orders.Add(new Order
                    {
                        order_id = orderId,
                        order_date = (DateTime)reader["order_date"],
                        total_amount = (decimal)reader["total_amount"],
                        status = (string)reader["status"]
                    });
                    shipmentAddresses[orderId] = (string)reader["shipment_address"];
                    productCounts[orderId] = (int)reader["NumberOfProducts"];
                }
                ViewBag.ShipmentAddresses = shipmentAddresses;
                ViewBag.ProductCounts = productCounts;
                return View(orders);
            }
        }



        // GET: Order Details
        public ActionResult OrderDetails(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        oi.order_item_id,
                        oi.quantity,
                        oi.price,
                        oi.product_id
                    FROM
                        Order_Items oi
                        JOIN Products p ON oi.product_id = p.product_id
                    WHERE
                        oi.order_id = @OrderId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Order_Items> orderDetails = new List<Order_Items>();
                while (reader.Read())
                    orderDetails.Add(new Order_Items
                    {
                        order_item_id = (int)reader["order_item_id"],
                        quantity = (int)reader["quantity"],
                        price = (decimal)reader["price"],
                        Product = new ProductController().GetProductByID((int)reader["product_id"])
                    }); ;
                return View(orderDetails);
            }
        }


        private decimal CalculateTotalAmount(int userId, SqlConnection con)
        {
            string query = @"
                SELECT
                    SUM(c.quantity * p.price)
                FROM
                    Cart c
                    JOIN Products p ON c.product_id = p.product_id
                WHERE
                    c.user_id = @UserId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            return (decimal)cmd.ExecuteScalar();
        }
    }

}