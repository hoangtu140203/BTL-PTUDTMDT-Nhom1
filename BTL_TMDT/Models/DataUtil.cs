using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BTL_TMDT.Models
{
    public class DataUtil
    {
        SqlConnection con;
        public DataUtil()
        {
            string sql = @"Data Source=DESKTOP-OEMIQ40\SQLEXPRESS;Initial Catalog=thuongmaidientudb;Integrated Security=True";
            con = new SqlConnection(sql);
        }
		// count and revenue of day
        public List<string> GetDateTimeDay()
        {
            List<string> list = new List<string>();
            con.Open();
            string sqltext = @"WITH DateRange AS (
					SELECT CAST(GETDATE() AS DATE) AS OrderDate
					UNION ALL
					SELECT DATEADD(DAY, -1, OrderDate)
					FROM DateRange
					WHERE OrderDate > DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
				)
				SELECT 
					d.OrderDate,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					DateRange d
				LEFT JOIN 
					Orders o ON CAST(o.order_date AS DATE) = d.OrderDate
				GROUP BY 
					d.OrderDate
				ORDER BY 
					d.OrderDate
				OPTION (MAXRECURSION 7);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
			while (rd.Read())
			{
				string day = rd["OrderDate"].ToString();
				list.Add(day);
			}
			con.Close();
			Console.WriteLine(list);
			return list;
        }

		public List<int> GetCountDay()
		{
			List<int> list = new List<int>();
            con.Open();
            string sqltext = @"WITH DateRange AS (
					SELECT CAST(GETDATE() AS DATE) AS OrderDate
					UNION ALL
					SELECT DATEADD(DAY, -1, OrderDate)
					FROM DateRange
					WHERE OrderDate > DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
				)
				SELECT 
					d.OrderDate,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					DateRange d
				LEFT JOIN 
					Orders o ON CAST(o.order_date AS DATE) = d.OrderDate
				GROUP BY 
					d.OrderDate
				ORDER BY 
					d.OrderDate
				OPTION (MAXRECURSION 7);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                int count = (int)rd["OrderCount"];
                list.Add(count);
            }
            con.Close();
            return list;
        }

		public List<decimal> GetRevenueDay()
		{
			List<decimal> list = new List<decimal>();
			con.Open();
			string sqltext = @"WITH DateRange AS (
					SELECT CAST(GETDATE() AS DATE) AS OrderDate
					UNION ALL
					SELECT DATEADD(DAY, -1, OrderDate)
					FROM DateRange
					WHERE OrderDate > DATEADD(DAY, -6, CAST(GETDATE() AS DATE))
				)
				SELECT 
					d.OrderDate,
					ISNULL(SUM(o.total_amount), 0) AS TotalRevenue
				FROM 
					DateRange d
				LEFT JOIN 
					Orders o ON CAST(o.order_date AS DATE) = d.OrderDate
				GROUP BY 
					d.OrderDate
				ORDER BY 
					d.OrderDate
				OPTION (MAXRECURSION 7);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                decimal revenue = (decimal)rd["TotalRevenue"];
                list.Add(revenue);
            }
            con.Close();
            return list;
        }


        // count and revenue of month
        public List<string> GetDateTimeMonth()
        {
            List<string> list = new List<string>();
            con.Open();
            string sqltext = @"				WITH MonthRange AS (
					SELECT DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart
					UNION ALL
					SELECT DATEADD(MONTH, 1, MonthStart)
					FROM MonthRange
					WHERE DATEADD(MONTH, 1, MonthStart) <= DATEFROMPARTS(YEAR(GETDATE()), 12, 1)
				)
				SELECT 
					FORMAT(mr.MonthStart, 'yyyy-MM') AS Month,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					MonthRange mr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = YEAR(mr.MonthStart) AND MONTH(o.order_date) = MONTH(mr.MonthStart)
				GROUP BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				ORDER BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				OPTION (MAXRECURSION 12);	";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                string day = rd["Month"].ToString();
                list.Add(day);
            }
            con.Close();
            return list;
        }

        public List<int> GetCountMonth()
        {
            List<int> list = new List<int>();
            con.Open();
            string sqltext = @"				WITH MonthRange AS (
					SELECT DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart
					UNION ALL
					SELECT DATEADD(MONTH, 1, MonthStart)
					FROM MonthRange
					WHERE DATEADD(MONTH, 1, MonthStart) <= DATEFROMPARTS(YEAR(GETDATE()), 12, 1)
				)
				SELECT 
					FORMAT(mr.MonthStart, 'yyyy-MM') AS Month,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					MonthRange mr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = YEAR(mr.MonthStart) AND MONTH(o.order_date) = MONTH(mr.MonthStart)
				GROUP BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				ORDER BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				OPTION (MAXRECURSION 12);	";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                int count = (int)rd["OrderCount"];
                list.Add(count);
            }
            con.Close();
            return list;
        }

        public List<decimal> GetRevenueMonth()
        {
            List<decimal> list = new List<decimal>();
            con.Open();
            string sqltext = @"WITH MonthRange AS (
					SELECT DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AS MonthStart
					UNION ALL
					SELECT DATEADD(MONTH, 1, MonthStart)
					FROM MonthRange
					WHERE DATEADD(MONTH, 1, MonthStart) <= DATEFROMPARTS(YEAR(GETDATE()), 12, 1)
				)
				-- Truy vấn chính
				SELECT 
					FORMAT(mr.MonthStart, 'yyyy-MM') AS Month,
					ISNULL(SUM(o.total_amount), 0) AS TotalRevenue
				FROM 
					MonthRange mr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = YEAR(mr.MonthStart) AND MONTH(o.order_date) = MONTH(mr.MonthStart)
				GROUP BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				ORDER BY 
					FORMAT(mr.MonthStart, 'yyyy-MM')
				OPTION (MAXRECURSION 12);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                decimal revenue = (decimal)rd["TotalRevenue"];
                list.Add(revenue);
            }
            con.Close();
            return list;
        }


        // count and revenue of years
        public List<string> GetDateTimeYear()
        {
            List<string> list = new List<string>();
            con.Open();
            string sqltext = @"				WITH YearRange AS (
					SELECT YEAR(GETDATE()) AS Year
					UNION ALL
					SELECT Year - 1
					FROM YearRange
					WHERE Year > YEAR(GETDATE()) - 4
				)
				SELECT 
					yr.Year,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					YearRange yr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = yr.Year
				GROUP BY 
					yr.Year
				ORDER BY 
					yr.Year
				OPTION (MAXRECURSION 5);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                string day = rd["Year"].ToString();
                list.Add(day);
            }
            con.Close();
            return list;
        }

        public List<int> GetCountYear()
        {
            List<int> list = new List<int>();
            con.Open();
            string sqltext = @"				WITH YearRange AS (
					SELECT YEAR(GETDATE()) AS Year
					UNION ALL
					SELECT Year - 1
					FROM YearRange
					WHERE Year > YEAR(GETDATE()) - 4
				)
				SELECT 
					yr.Year,
					ISNULL(COUNT(o.order_id), 0) AS OrderCount
				FROM 
					YearRange yr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = yr.Year
				GROUP BY 
					yr.Year
				ORDER BY 
					yr.Year
				OPTION (MAXRECURSION 5);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                int count = (int)rd["OrderCount"];
                list.Add(count);
            }
            con.Close();
            return list;
        }

        public List<decimal> GetRevenueYear()
        {
            List<decimal> list = new List<decimal>();
            con.Open();
            string sqltext = @"WITH YearRange AS (
					SELECT YEAR(GETDATE()) AS Year
					UNION ALL
					SELECT Year - 1
					FROM YearRange
					WHERE Year > YEAR(GETDATE()) - 4
				)
				-- Truy vấn chính
				SELECT 
					yr.Year,
					ISNULL(SUM(o.total_amount), 0) AS TotalRevenue
				FROM 
					YearRange yr
				LEFT JOIN 
					Orders o ON YEAR(o.order_date) = yr.Year
				GROUP BY 
					yr.Year
				ORDER BY 
					yr.Year
				OPTION (MAXRECURSION 5);";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                decimal revenue = (decimal)rd["TotalRevenue"];
                list.Add(revenue);
            }
            con.Close();
            return list;
        }

		public List<ProductContainer> GetSellingProduct()
		{
			List <ProductContainer> list = new List<ProductContainer>();
			con.Open();
			string sqltext = @"WITH CategorySales AS (
		SELECT TOP 5
			c.category_name,
			p.product_name,
			p.category_id,
			SUM(oi.quantity) AS TotalQuantitySold
		FROM 
			Categories c
		JOIN 
			Products p ON c.category_id = p.category_id
		JOIN 
			Order_Items oi ON p.product_id = oi.product_id
		GROUP BY 
			c.category_name, p.product_name, p.category_id
	),
	MaxCategorySales AS (
		SELECT 
			category_id,
			MAX(TotalQuantitySold) AS MaxQuantity
		FROM 
			CategorySales
		GROUP BY 
			category_id
	)
	SELECT 
		cs.category_name,
		cs.product_name,
		cs.TotalQuantitySold
	FROM 
		CategorySales cs
	JOIN 
		MaxCategorySales mcs ON cs.category_id = mcs.category_id AND cs.TotalQuantitySold = mcs.MaxQuantity
	ORDER BY 
		cs.category_name, cs.TotalQuantitySold DESC;";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                ProductContainer pc = new ProductContainer();
				pc.category_name = rd["category_name"].ToString();
				pc.product_name = rd["product_name"].ToString();
				pc.totalquantitysold = (int)rd["TotalQuantitySold"];
                list.Add(pc);
            }
            con.Close();
            return list;
        }


        public List<ProductContainer> GetUnsoldProduct()
        {
            List<ProductContainer> list = new List<ProductContainer>();
            con.Open();
            string sqltext = @"
	-- Top 5 sản phẩm có TotalQuantitySold thấp nhất
SELECT TOP 5
    c.category_name,
    p.product_name,
    ISNULL(SUM(oi.quantity), 0) AS TotalQuantitySold
FROM 
    Categories c
JOIN 
    Products p ON c.category_id = p.category_id
LEFT JOIN 
    Order_Items oi ON p.product_id = oi.product_id
GROUP BY
    c.category_name, p.product_name
ORDER BY 
    ISNULL(SUM(oi.quantity), 0) ASC;";
            SqlCommand sql = new SqlCommand(sqltext, con);
            SqlDataReader rd = sql.ExecuteReader();
            while (rd.Read())
            {
                ProductContainer pc = new ProductContainer();
                pc.category_name = rd["category_name"].ToString();
                pc.product_name = rd["product_name"].ToString();
                pc.totalquantitysold = (int)rd["TotalQuantitySold"];
                list.Add(pc);
            }
            con.Close();
            return list;
        }
    }
}