USE master
GO

--SP_WHO
--KILL 61

-- Check and drop the database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'thuongmaidientudb')
BEGIN
    DROP DATABASE thuongmaidientudb;
END
GO

-- Create the database
CREATE DATABASE thuongmaidientudb;
GO

-- Use the created database
USE thuongmaidientudb;
GO

-- Drop the tables if they exist
IF OBJECT_ID('Order_Items', 'U') IS NOT NULL DROP TABLE Order_Items;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Shipments', 'U') IS NOT NULL DROP TABLE Shipments;
IF OBJECT_ID('Cart', 'U') IS NOT NULL DROP TABLE Cart;
IF OBJECT_ID('Wishlist', 'U') IS NOT NULL DROP TABLE Wishlist;
IF OBJECT_ID('Product_Images', 'U') IS NOT NULL DROP TABLE Product_Images;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- Create the Users table
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name NVARCHAR(50) NOT NULL,
    last_name NVARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    role NVARCHAR(50)
);

-- Create the Categories table
CREATE TABLE Categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    category_name NVARCHAR(100) NOT NULL
);

-- Create the Products table
CREATE TABLE Products (
    product_id INT IDENTITY(1,1) PRIMARY KEY,
    category_id INT NOT NULL FOREIGN KEY REFERENCES Categories(category_id),
    product_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(2000),
    price DECIMAL(16, 2),
    discount_price DECIMAL(16, 2),
    stock INT,
    brand NVARCHAR(255),
    is_new BIT
);

-- Create the Product_Images table
CREATE TABLE Product_Images (
    image_id INT IDENTITY(1,1) PRIMARY KEY,
    image_url VARCHAR(255) NOT NULL,
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(product_id)
);

-- Create the Wishlist table
CREATE TABLE Wishlist (
    wishlist_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(product_id)
);

-- Create the Cart table
CREATE TABLE Cart (
    cart_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(product_id),
    quantity INT
);

-- Create the Shipments table
CREATE TABLE Shipments (
    shipment_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    recipient_first_name NVARCHAR(50) NOT NULL,
    recipient_last_name NVARCHAR(50) NOT NULL,
    recipient_phone VARCHAR(20),
    shipment_address NVARCHAR(255),
    shipment_city NVARCHAR(50),
    shipment_country NVARCHAR(50),
    shipment_zip_code NVARCHAR(20)
);

-- Create the Orders table
CREATE TABLE Orders (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    shipment_id INT NOT NULL FOREIGN KEY REFERENCES Shipments(shipment_id),
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    order_date DATETIME,
    total_amount DECIMAL(16, 2),
    status NVARCHAR(50),
    payment_method NVARCHAR(50),
    order_note CHAR(10)
);

-- Create the Order_Items table
CREATE TABLE Order_Items (
    order_item_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL FOREIGN KEY REFERENCES Orders(order_id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(product_id),
    quantity INT,
    price DECIMAL(16, 2)
);





--user
INSERT INTO Users (first_name, last_name, email, password, phone, role)
VALUES 
(N'Mạnh', N'Trương Công', 'tcm@nhom1.com', 'tcm123', '1234567890', 'admin'),
(N'Thông', N'Nguyễn Bá', 'nbt@nhom1.com', 'nbt123', '0987654321', 'admin'),
(N'Vũ', N'Trần Xuân', 'txv@nhom1.com', 'txv123', '1122334455', 'customer'),
(N'Tú', N'Hoàng Anh', 'hat@nhom1.com', 'hat123', '0974871548', 'customer');

-- Insert data into Categories
INSERT INTO Categories (category_name)
VALUES 
(N'Điện thoại'),
(N'Laptop'),
(N'Máy ảnh'),
(N'Phụ kiện');

--dienthoai
INSERT INTO Products (category_id, product_name, description, price, discount_price, stock, brand, is_new)
VALUES 
(1, N'iPhone 15 Pro Max 256GB', N'iPhone 15 Pro Max sở hữu màn hình Super Retina XDR OLED 6.7 inches với độ phân giải 2796 x 1290 pixels, cung cấp trải nghiệm hình ảnh sắc nét, chân thực. So với các phiên bản tiền nhiệm, thế hệ iPhone 15 bản Pro Max đảm bảo mang tới hiệu năng mạnh mẽ với sự hỗ trợ của chipset Apple A17 Pro, cùng bộ nhớ ấn tượng. Đặc biệt hơn, điện thoại iPhone 15 ProMax mới này còn được đánh giá cao với camera sau 48MP và camera trước 12MP, hỗ trợ chụp ảnh với độ rõ nét cực đỉnh.', 29390000, 27390000, 100, 'Apple', 1),
(1, N'iPhone 12 Pro Max 256GB', N'iPhone 12 Pro Max 256GB VN/A – Dung lượng lưu trữ nâng cấp Với một cấu hình hiệu năng có thể nói mạnh nhất trong năm 2020 cũng như duy trì được sự mạnh mẽ này cho đến nhiều năm nữa. Đòi hỏi khả năng lưu trữ của iPhone phải cao để tránh thiếu dung lượng. iPhone 12 Pro Max 256GB chính hãng (VN/A) sẽ là một sự lựa chọn đúng nhất cho bạn. Có thể nói với việc trang bị chip Apple A14 Bionic chiếc điện thoại iPhone 12 Pro Max bản 256GB sẽ mang đến cho bạn một tốc độ xử lý khá nhanh kèm theo đó là bộ nhớ ram lên đến 6GB cho khả năng đa nhiệm khá tốt.', 27590000, 25390000, 200, 'Apple', 0),
(1, N'Samsung Galaxy S24 Ultra 12GB 256GB', N'Samsung S24 Ultra là siêu phẩm smartphone đỉnh cao mở đầu năm 2024 đến từ nhà Samsung với chip Snapdragon 8 Gen 3 For Galaxy mạnh mẽ, công nghệ tương lai Galaxy AI cùng khung viền Titan đẳng cấp hứa hẹn sẽ mang tới nhiều sự thay đổi lớn về mặt thiết kế và cấu hình. SS Galaxy S24 bản Ultra sở hữu màn hình 6.8 inch Dynamic AMOLED 2X tần số quét 120Hz. Máy cũng sở hữu camera chính 200MP, camera zoom quang học 50MP, camera tele 10MP và camera góc siêu rộng 12MP.', 29990000, 29490000, 150, 'Samsung', 1),
(1, N'Samsung Galaxy S23 FE 5G 8GB 128GB', N'Samsung S23 FE sở hữu màn hình Dynamic AMOLED 2X 6.4 inch, tần số quét 120Hz đi cùng chip Exynos 2200 8 nhân tạo độ mượt mà, thoải mái khi sử dụng. Bên cạnh đó, với mức pin 4.500 mAh, giúp người dùng có thể tha hồ đọc báo, lướt web cả ngày dài, kết hợp sạc nhanh 25W, tiết kiệm thời gian sạc. Khả năng chụp ảnh của S23 FE 5G cũng được đánh giá cao với camera chính 50MP, quay video lên đến 8K 4320p@24fps.', 12890000, 10890000, 150, 'Samsung', 0),
(1, N'Xiaomi 14 Ultra 5G (16GB 512GB)', N'Tại sự kiện MWC 2024, Xiaomi đã giới thiệu tổng cộng bốn cấu hình của dòng Mi 14 Ultra. Tuy nhiên trái ngược với phiên bản nội địa Trung Quốc, phiên bản quốc tế được đăng tải trên website của Xiaomi sẽ chỉ có duy nhất một cấu hình trang bị chip Snapdragon 8 Gen 3 cùng 16GB RAM LPDDR5 và 512GB dung lượng bộ nhớ chuẩn UFS 4.0.', 32990000, 26990000, 150, 'Xiaomi', 1);

INSERT INTO Product_Images (image_url, product_id)
VALUES 
('apple1_1.png', 1),
('apple1_2.png', 1),
('apple1_2.png', 1),
('apple2_1.png', 2),
('apple2_2.png', 2),
('apple2_2.png', 2),
('ss1_1.png', 3),
('ss1_2.png', 3),
('ss1_3.png', 3),
('ss2_1.png', 4),
('ss2_2.png', 4),
('ss2_3.png', 4),
('xia1_1.png', 5),
('xia1_2.png', 5),
('xia1_3.png', 5);

--Laptop
INSERT INTO Products (category_id, product_name, description, price, discount_price, stock, brand, is_new)
VALUES 
(2, N'Apple MacBook Air M1 256GB 2020', N'Macbook Air M1 là dòng sản phẩm có thiết kế mỏng nhẹ, sang trọng và tinh tế cùng với đó là giá thành phải chăng nên MacBook Air đã thu hút được đông đảo người dùng yêu thích và lựa chọn. Đây cũng là một trong những phiên bản Macbook Air mới nhất mà khách hàng không thể bỏ qua khi đến với CellphoneS. Dưới đây là chi tiết về thiết kế, cấu hình của máy.', 18590000, 16590000, 100, 'Apple', 0),
(2, N'Macbook Pro 14 M3 Pro 18GB - 512GB', N'Macbook Pro 14 inch M3 Pro 2023 18GB/512GB có độ phân giải 3.024 x 1.964 pixels, độ sáng lên tới 1.600 nits, hỗ trợ tần số quét 120Hz xử lý hình ảnh cực mượt. Đặc biệt, sản phẩm Macbook Pro M3 năm 2023 trang bị con chip Apple M3 Pro, đi kèm với 18GB RAM và 512GB bộ nhớ trong. ', 49490000, 47490000, 100, 'Apple', 1),
(2, N'Macbook Pro 16 inch M1 Max 10 CPU - 32 GPU 32GB 1TB 2021 ', N'Không chỉ là điểm nhận biết trên các thiết bị smartphone, hiện nay tai thỏ đã xuất hiện trên thế hệ Macbook mới nhất. Macbook Pro M1 Max với thiết kế độc đáo, màn hình chất lượng mang lại trải nghiệm vượt  trội. Máy tính Macbook Pro 16 inch 2021 được trang bị cấu hình cực khủng với chip Apple M1 Max với 10CPU, 32GPU đi kèm dung lượng lên đến RAM 32GB và bộ nhớ SSD 1TB mang lại hiệu suất vượt trội.', 51990000, 49990000, 100, 'Apple', 1);

INSERT INTO Product_Images (image_url, product_id)
VALUES 
('lla1_1.png', 6),
('lla1_2.png', 6),
('lla1_2.png', 6),
('lla2_1.png', 7),
('lla2_2.png', 7),
('lla2_2.png', 7),
('lla3_1.png', 8),
('lla3_2.png', 8),
('lla3_3.png', 8);


--Máy ảnh
INSERT INTO Products (category_id, product_name, description, price, discount_price, stock, brand, is_new)
VALUES 
(3, N'Máy ảnh kỹ thuật số Sony ZV1 II', N'Máy ảnh kỹ thuật số Sony ZV-1 II sở hữu ưu điểm ở thiết kế nhỏ gọn giúp bạn có thể mang theo dễ dàng trong những chuyến du lịch hay công tác xa. Hơn nữa, sản phẩm máy ảnh Sony này cũng được trang bị ống kính zoom góc rộng hỗ trợ người dùng căn khung linh hoạt và lưu lại mọi khoảnh khắc vô cùng sắc nét. ', 22990000, 19990000, 100, 'Sony', 1),
(3, N'Máy ảnh kỹ thuật số Sony ZV-1F', N'Máy ảnh kỹ thuật số Sony ZV1F là công cụ hỗ trợ đắc lực cho những ai đang làm vlogger hoặc nhà sáng tạo nội dung trực tuyến. Sở hữu thiết kế nhỏ gọn “bỏ túi” nhưng lại bắt trọn được mọi khung hình với ống kính góc siêu rộng 20 mm. Do vậy, hãy cùng CellphoneS khám phá nhiều hơn về tính năng của máy chụp ảnh Sony thế hệ mới này nhé!', 13990000, 10590000, 100, 'Sony', 0);

INSERT INTO Product_Images (image_url, product_id)
VALUES 
('cam1_1.png', 9),
('cam1_2.png', 9),
('cam1_2.png', 9),
('cam2_1.png', 10),
('cam2_2.png', 10),
('cam2_2.png', 10);

--Phụ kiện
INSERT INTO Products (category_id, product_name, description, price, discount_price, stock, brand, is_new)
VALUES 
(4, N'Tai nghe chụp tai Gaming Sony Inzone H3', N'Sony INZONE H3 được thiết kế với cấu trúc âm học đối xứng, vừa đủ để giúp âm thanh trở nên sống động hơn rất nhiều lần. Nhờ được kích hoạt từ phần mềm PC INZONE Hub, Tai nghe có thể tái tạo tín hiệu thành âm thanh vòm 7.1 kênh.', 10900000, 1000000, 100, 'Sony', 0),
(4, N'Tai nghe Bluetooth Apple AirPods Pro 2 2023 USB-C', N'Airpods Pro 2 Type-C với công nghệ khử tiếng ồn chủ động mang lại khả năng khử ồn lên gấp 2 lần mang lại trải nghiệm nghe - gọi và trải nghiệm âm nhạc ấn tượng. Cùng với đó, điện thoại còn được trang bị công nghệ âm thanh không gian giúp trải nghiệm âm nhạc thêm phần sống động. Airpods Pro 2 Type-C với cổng sạc Type C tiện lợi cùng viên pin mang lại thời gian trải nghiệm lên đến 6 giờ tiện lợi.', 61900000, 5580000, 100, 'Apple', 1);


INSERT INTO Product_Images (image_url, product_id)
VALUES 
('tn1_1.png', 11),
('tn1_2.png', 11),
('tn1_2.png', 11),
('tn2_1.png', 12),
('tn2_2.png', 12),
('tn2_2.png', 12);

-- Insert data into Wishlist
INSERT INTO Wishlist (user_id, product_id)
VALUES 
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(1,10);

 --Insert data into Cart
INSERT INTO Cart (user_id, product_id, quantity)
VALUES 
(1, 1, 2),
(2, 2, 1),
(3, 3, 3),
(4, 10, 2),
(4, 1, 1);


-- Insert data into Shipments
INSERT INTO Shipments (user_id, recipient_first_name, recipient_last_name, recipient_phone, shipment_address, shipment_city, shipment_country, shipment_zip_code)
VALUES 
(1, N'Mạnh', N'Trương Công', N'1234567890', N'123 Bắc Từ Liêm', N'Hà Nội', N'Việt Nam', '12345'),
(2, N'Mạnh', N'Trương Công', N'1234567890', N'456 Nam Từ Liêm', N'Hà Nội', N'Việt Nam', '67890'),
(3, N'Tú', N'Hoàng Anh', N'0974871548', N'789 Thanh Xuân', N'Hà Nội', N'Việt Nam', '11223'),
(4, N'Thông', N'Nguyễn Bá', N'0987654321', N'789 Cầu Giấy', N'Hà Nội', N'Việt Nam', '11229');

-- Insert data into Orders
INSERT INTO Orders (shipment_id, user_id, order_date, total_amount, status, payment_method, order_note)
VALUES 
(1, 1, '2024-06-06 10:00:00', 54780000, 'Shipped', 'Credit Card', 'Note1'),
(2, 2, '2024-06-07 11:00:00', 25390000, 'Pending', 'PayPal', 'Note2'),
(3, 3, '2024-06-08 12:00:00', 88470000, 'Delivered', 'Bank Transfer', 'Note3'),
(4, 3, '2024-06-08 12:00:00', 48570000, 'Shipped', 'Bank Transfer', 'Note4');
-- Insert data into Order_Items
INSERT INTO Order_Items (order_id, product_id, quantity, price)
VALUES 
(1, 1, 2, 27390000),
(2, 2, 1, 25390000),
(3, 3, 3, 29490000),
(4, 10, 2,10590000),
(4, 1, 1, 27390000);
