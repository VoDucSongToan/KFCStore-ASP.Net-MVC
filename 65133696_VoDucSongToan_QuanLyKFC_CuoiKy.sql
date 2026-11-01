CREATE DATABASE KFC_Store;
GO
USE KFC_Store;
GO

-- =============================================
-- 2. TẠO CẤU TRÚC BẢNG (TABLE SCHEMAS)
-- =============================================

-- 2.1. Bảng Tài Khoản
CREATE TABLE Tai_Khoan (
    MaTK INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(150),
    Email NVARCHAR(150) UNIQUE,
    MatKhau NVARCHAR(100),
    DiaChi NVARCHAR(255),
    SoDT NVARCHAR(20),
    VaiTro NVARCHAR(50), -- QuanLy, NhanVien, NguoiDung
    TrangThai BIT DEFAULT 1
);

-- 2.2. Bảng Danh Mục Món
CREATE TABLE Loai_Mon (
    MaLoai INT PRIMARY KEY IDENTITY(1,1),
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);

-- 2.3. Bảng Món Ăn
CREATE TABLE Mon_An (
    MaMon INT PRIMARY KEY IDENTITY(1,1),
    MaLoai INT FOREIGN KEY REFERENCES Loai_Mon(MaLoai),
    TenMon NVARCHAR(150) NOT NULL,
    MoTa NVARCHAR(255),
    Gia DECIMAL(18,0) NOT NULL, 
    HinhAnh NVARCHAR(255),
    TrangThai BIT DEFAULT 1
);

-- 2.4. Bảng Khuyến Mãi (Cho phép giảm giá 0% - Freeship)
CREATE TABLE Khuyen_Mai (
    MaKM INT PRIMARY KEY IDENTITY(1,1),
    TenKM NVARCHAR(150),
    NoiDung NVARCHAR(255),
    PhanTramGiam INT CHECK (PhanTramGiam >= 0 AND PhanTramGiam <= 100), 
    NgayBatDau DATE,
    NgayKetThuc DATE,
    TrangThai BIT DEFAULT 1
);

-- 2.5. Bảng Món áp dụng Khuyến Mãi
CREATE TABLE Mon_Khuyen_Mai (
    MaMon INT FOREIGN KEY REFERENCES Mon_An(MaMon),
    MaKM INT FOREIGN KEY REFERENCES Khuyen_Mai(MaKM),
    PRIMARY KEY (MaMon, MaKM)
);

-- 2.6. Bảng Đơn Hàng
CREATE TABLE Don_Hang (
    MaDH INT PRIMARY KEY IDENTITY(1,1),
    MaTK INT FOREIGN KEY REFERENCES Tai_Khoan(MaTK),
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,0),
    TrangThai NVARCHAR(50), -- ChoXacNhan, DaXacNhan, DangGiao, HoanThanh, Huy
    DiaChiGiao NVARCHAR(255),
    GhiChu NVARCHAR(255)
);

-- 2.7. Bảng Chi Tiết Đơn Hàng
CREATE TABLE Chi_Tiet_Don_Hang (
    MaDH INT FOREIGN KEY REFERENCES Don_Hang(MaDH),
    MaMon INT FOREIGN KEY REFERENCES Mon_An(MaMon),
    SoLuong INT,
    DonGia DECIMAL(18,0),
    PRIMARY KEY (MaDH, MaMon)
);

-- 2.8. Bảng Giao Hàng (Shipper)
CREATE TABLE Giao_Hang (
    MaGH INT PRIMARY KEY IDENTITY(1,1),
    MaDH INT FOREIGN KEY REFERENCES Don_Hang(MaDH),
    MaTK INT FOREIGN KEY REFERENCES Tai_Khoan(MaTK), -- Shipper ID
    TrangThai NVARCHAR(50),
    ThoiGianCapNhat DATETIME DEFAULT GETDATE()
);

-- 2.9. Bảng Chi Nhánh (Chứa Link Google Maps thực tế)
CREATE TABLE Chi_Nhanh (
    MaCN INT PRIMARY KEY IDENTITY(1,1),
    TenCN NVARCHAR(150),
    DiaChi NVARCHAR(255),
    SoDT NVARCHAR(50),
    GioMoCua NVARCHAR(50) DEFAULT '09:00 - 22:00',
    LinkMap NVARCHAR(MAX) NULL
);

-- 2.10. Bảng Liên Hệ & Phản Hồi (Hỗ trợ Admin trả lời)
CREATE TABLE LienHe (
    MaLH INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NULL FOREIGN KEY REFERENCES Tai_Khoan(MaTK),
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(20),
    TieuDe NVARCHAR(200),
    NoiDung NTEXT,
    NgayGui DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 0,            -- 0: Chưa xem, 1: Đã xem
    PhanHoi NTEXT NULL,                 -- Nội dung Admin trả lời
    NgayPhanHoi DATETIME NULL,          -- Ngày trả lời
    NguoiPhanHoi INT NULL FOREIGN KEY REFERENCES Tai_Khoan(MaTK) -- Admin nào trả lời
);
GO

-- =============================================
-- 3. NẠP DỮ LIỆU MẪU (SEED DATA)
-- =============================================

-- 3.1. Dữ liệu Tài Khoản
INSERT INTO Tai_Khoan (HoTen, Email, MatKhau, DiaChi, SoDT, VaiTro) VALUES
(N'Quản lý A', 'admin@kfc.com','123','HCM','0901','QuanLy'),
(N'Nhân viên bán hàng 1','nvbh1@kfc.com','123','HCM','0902','NhanVienBanHang'),
(N'Nhân viên bán hàng 2','nvbh2@kfc.com','123','HN','0903','NhanVienBanHang'),
(N'Shipper 1','ship1@kfc.com','123','HCM','0904','NhanVienGiaoHang'),
(N'Shipper 2','ship2@kfc.com','123','HCM','0905','NhanVienGiaoHang'),
(N'Khách hàng 1','user1@mail.com','123','HCM','0906','NguoiDung'),
(N'Khách hàng 2','user2@mail.com','123','HCM','0907','NguoiDung'),
(N'Khách hàng mẫu','user3@example.com','123',N'Địa chỉ mẫu','0900000000','NguoiDung');

-- 3.2. Dữ liệu Loại Món
INSERT INTO Loai_Mon (TenLoai, MoTa) VALUES
(N'Ưu đãi', N'Khuyến mãi và ưu đãi online'),
(N'Món mới', N'Sản phẩm mới ra mắt'),
(N'Combo', N'Combo tiết kiệm'),
(N'Các món gà', N'Gà rán – gà quay'),
(N'Cơm - Mì Ý - Burger', N'Món ăn chính'),
(N'Giải khát - Tráng miệng', N'Nước uống và tráng miệng');

-- 3.3. Dữ liệu Món Ăn (Danh sách đầy đủ)
INSERT INTO Mon_An (MaLoai, TenMon, MoTa, Gia, HinhAnh) VALUES
-- Ưu đãi
(1, N'Combo ưu đãi 69K', N'1 gà + khoai + nước', 69000, 'uudai1.jpg'),
(1, N'Ưu đãi online 99K', N'Chỉ bán online', 99000, 'uudai2.jpg'),
(1, N'Combo trưa tiết kiệm', N'Giá rẻ buổi trưa', 59000, 'uudai3.jpg'),
(1, N'Combo sinh viên', N'Ưu đãi SV', 55000, 'uudai4.jpg'),
(1, N'Combo tối 2 người', N'Giảm giá buổi tối', 129000, 'uudai5.jpg'),
(1, N'Combo app KFC', N'Đặt app giảm giá', 89000, 'uudai6.jpg'),
(1, N'Combo gà + nước', N'Giá tốt', 75000, 'uudai7.jpg'),
(1, N'Combo khoai lớn', N'Khoai + nước', 49000, 'uudai8.jpg'),
(1, N'Combo gia đình nhỏ', N'Ưu đãi gia đình', 199000, 'uudai9.jpg'),
(1, N'Combo cuối tuần', N'Ưu đãi đặc biệt', 159000, 'uudai10.jpg'),
-- Món mới
(2, N'Gà sốt phô mai', N'Món mới', 45000, 'monmoi1.jpg'),
(2, N'Gà sốt tiêu đen', N'Mới ra mắt', 47000, 'monmoi2.jpg'),
(2, N'Burger gà sốt cay', N'Burger mới', 59000, 'monmoi3.jpg'),
(2, N'Cơm gà BBQ', N'Cơm mới', 69000, 'monmoi4.jpg'),
(2, N'Mì Ý sốt kem', N'Mì mới', 59000, 'monmoi5.jpg'),
(2, N'Gà quay mật ong', N'Phiên bản mới', 99000, 'monmoi6.jpg'),
(2, N'Khoai lắc phô mai', N'Khoai mới', 39000, 'monmoi7.jpg'),
(2, N'Kem vị chocolate', N'Món mới', 29000, 'monmoi8.jpg'),
(2, N'Nước chanh đào', N'Nước mới', 25000, 'monmoi9.jpg'),
(2, N'Salad gà giòn', N'Món mới', 45000, 'monmoi10.jpg'),
-- Combo
(3, N'Combo 1 người', N'Gà + khoai + nước', 79000, 'combo1.jpg'),
(3, N'Combo 2 người', N'2 gà + khoai', 149000, 'combo2.jpg'),
(3, N'Combo gia đình', N'4 người ăn', 299000, 'combo3.jpg'),
(3, N'Combo gà quay', N'Gà quay đặc biệt', 189000, 'combo4.jpg'),
(3, N'Combo burger', N'Burger + khoai', 99000, 'combo5.jpg'),
(3, N'Combo trẻ em', N'Kid meal', 55000, 'combo6.jpg'),
(3, N'Combo tiết kiệm', N'Giá tốt', 89000, 'combo7.jpg'),
(3, N'Combo nhóm bạn', N'3–4 người', 259000, 'combo8.jpg'),
(3, N'Combo buổi sáng', N'Ăn sáng', 49000, 'combo9.jpg'),
(3, N'Combo cuối tuần', N'Gia đình', 329000, 'combo10.jpg'),
-- Các món gà
(4, N'Gà rán truyền thống', N'Giòn rụm', 35000, 'ga1.jpg'),
(4, N'Gà rán cay', N'Cay nồng', 36000, 'ga2.jpg'),
(4, N'Gà rán không cay', N'Không cay', 34000, 'ga3.jpg'),
(4, N'Gà quay mật ong', N'Đặc biệt', 99000, 'ga4.jpg'),
(4, N'Gà quay tiêu', N'Đậm vị', 95000, 'ga5.jpg'),
(4, N'Gà viên', N'Ăn vặt', 39000, 'ga6.jpg'),
(4, N'Cánh gà chiên', N'Giòn ngon', 42000, 'ga7.jpg'),
(4, N'Đùi gà chiên', N'Đùi to', 45000, 'ga8.jpg'),
(4, N'Ức gà chiên', N'Ít béo', 48000, 'ga9.jpg'),
(4, N'Gà popcorn', N'Ăn nhẹ', 35000, 'ga10.jpg'),
-- Cơm Burger
(5, N'Cơm gà sốt cay', N'Cơm phần', 65000, 'com1.jpg'),
(5, N'Cơm gà BBQ', N'Cơm BBQ', 69000, 'com2.jpg'),
(5, N'Mì Ý sốt bò', N'Mì Ý', 59000, 'mi1.jpg'),
(5, N'Mì Ý sốt kem', N'Mì kem', 59000, 'mi2.jpg'),
(5, N'Burger gà truyền thống', N'Burger', 59000, 'burger1.jpg'),
(5, N'Burger gà cay', N'Burger cay', 62000, 'burger2.jpg'),
(5, N'Burger phô mai', N'Phô mai', 65000, 'burger3.jpg'),
(5, N'Cơm gà quay', N'Cơm quay', 75000, 'com3.jpg'),
(5, N'Cơm gà giòn', N'Gà giòn', 68000, 'com4.jpg'),
(5, N'Burger double', N'2 miếng gà', 79000, 'burger4.jpg'),
-- Giải khát
(6, N'Pepsi lon', N'Nước ngọt', 19000, 'pepsi.jpg'),
(6, N'7Up lon', N'Nước ngọt', 19000, '7up.jpg'),
(6, N'Mirinda cam', N'Nước cam', 19000, 'mirinda.jpg'),
(6, N'Trà đào', N'Trà mát', 25000, 'tra1.jpg'),
(6, N'Trà chanh', N'Trà chanh', 25000, 'tra2.jpg'),
(6, N'Kem ly', N'Kem mát lạnh', 25000, 'kem1.jpg'),
(6, N'Kem ốc quế', N'Kem ốc quế', 20000, 'kem2.jpg'),
(6, N'Bánh trứng', N'Tráng miệng', 29000, 'banh1.jpg'),
(6, N'Pudding trứng', N'Ngọt nhẹ', 29000, 'banh2.jpg'),
(6, N'Nước suối', N'Chai nhỏ', 15000, 'nuoc.jpg');

-- 3.4. Dữ liệu Khuyến Mãi
INSERT INTO Khuyen_Mai (TenKM, NoiDung, PhanTramGiam, NgayBatDau, NgayKetThuc) VALUES
(N'Giảm 20% Combo', N'Áp dụng online', 20, '2025-01-01', '2026-01-31'),
(N'Mua 1 tặng 1', N'Gà rán', 50, '2025-02-01', '2026-02-15'),
(N'Giảm 10% món lẻ', N'Các món gọi thêm', 10, '2025-03-01', '2026-03-31'),
(N'Freeship đơn >150k', N'Online', 0, '2025-06-01', '2026-12-31');

-- 3.5. Dữ liệu Món Khuyến Mãi
INSERT INTO Mon_Khuyen_Mai (MaMon, MaKM) VALUES
(21,1),(22,1),(23,1),(24,1),(25,1),
(31,2),(32,2),(33,2),(34,2),
(51,3),(52,3),(53,3),(54,3);

-- 3.6. Dữ liệu Chi Nhánh (LINK MAP THỰC TẾ)
INSERT INTO Chi_Nhanh (TenCN, DiaChi, SoDT, GioMoCua, LinkMap) VALUES
-- KFC LÊ THÀNH PHƯƠNG
(N'KFC Lê Thành Phương', N'02 Lê Thành Phương, TP. Nha Trang', '0258 3811 999', '09:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3900.676453912956!2d109.18676637481352!3d12.24823498800755!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3170677a2337a723%3A0x62846f3456816226!2sKFC%20L%C3%AA%20Th%C3%A0nh%20Ph%C6%B0%C6%A1ng!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s'),
-- KFC NHA TRANG CENTER
(N'KFC Nha Trang Center', N'20 Trần Phú, TP. Nha Trang', '0258 3529 888', '09:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3900.730335805558!2d109.1931663148135!3d12.2393339880145!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317067784337b51f%3A0x50f9011781297b8!2sNha%20Trang%20Center!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s'),
-- KFC LOTTE NHA TRANG
(N'KFC Lotte Mart Nha Trang', N'58 Đường 23/10, TP. Nha Trang', '0258 3812 777', '08:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3900.6276846171545!2d109.17245631481355!3d12.2547899880026!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31706766418c3937%3A0x5a23053686851493!2sLotte%20Mart%20Nha%20Trang!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s'),
-- KFC NGUYỄN VĂN GIAI (HCM)
(N'KFC Nguyễn Văn Giai', N'123 Nguyễn Văn Giai, Q1, HCM', '028 3822 666', '10:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.426767664687!2d106.6967663148007!3d10.776789989322!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752f3855555555%3A0x5555555555555555!2sKFC!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s'),
-- KFC BÀ TRIỆU (HN)
(N'KFC Bà Triệu', N'291 Bà Triệu, Hai Bà Trưng, HN', '024 3974 555', '09:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.096767664687!2d105.8467663148007!3d21.026789989322!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab9bd9861ca1%3A0xe7887f7b72ca17a9!2sVincom%20Center%20B%C3%A0%20Tri%E1%BB%87u!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s'),
-- KFC ĐÀ NẴNG
(N'KFC Nguyễn Văn Linh', N'Nguyễn Văn Linh, Đà Nẵng', '0236 3650 444', '09:00 - 22:00',
'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3833.896767664687!2d108.2167663148007!3d16.056789989322!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x314219b555555555%3A0x5555555555555555!2sKFC!5e0!3m2!1svi!2s!4v1705030000000!5m2!1svi!2s');

-- 3.7. Dữ liệu Đơn Hàng Mẫu
INSERT INTO Don_Hang (MaTK, TongTien, TrangThai, DiaChiGiao) VALUES
(6, 0, N'ChoXacNhan', N'Quận 1'),
(7, 0, N'DaXacNhan', N'Quận 3'),
(8, 0, N'DangGiao', N'Nha Trang'),
(8, 0, N'HoanThanh', N'Nha Trang');

INSERT INTO Chi_Tiet_Don_Hang (MaDH, MaMon, SoLuong, DonGia)
SELECT 1, 1, 2, 69000 UNION ALL
SELECT 1, 4, 1, 35000 UNION ALL
SELECT 2, 2, 1, 45000 UNION ALL
SELECT 3, 3, 3, 79000;

INSERT INTO Giao_Hang (MaDH, MaTK, TrangThai) VALUES
(3, 4, N'DangGiao');
GO

-- =============================================
-- 4. CẬP NHẬT ĐƯỜNG DẪN ẢNH (IMAGES UPDATE)
-- =============================================
-- Ưu đãi
;WITH UuDaiCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 1)
UPDATE m SET HinhAnh = N'~/Content/imagesKFC_65133696/UuDai/UuDai' + CAST(u.STT AS NVARCHAR) + N'.jpg'
FROM Mon_An m JOIN UuDaiCTE u ON m.MaMon = u.MaMon;

-- Món mới
;WITH MonMoiCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 2)
UPDATE m SET HinhAnh = '~/Content/imagesKFC_65133696/MonMoi/MonMoi' + CAST(c.STT AS NVARCHAR) + '.jpg'
FROM Mon_An m JOIN MonMoiCTE c ON m.MaMon = c.MaMon;

-- Combo
;WITH ComboCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 3)
UPDATE m SET HinhAnh = '~/Content/imagesKFC_65133696/Combo/Combo' + CAST(c.STT AS NVARCHAR) + '.jpg'
FROM Mon_An m JOIN ComboCTE c ON m.MaMon = c.MaMon;

-- Món Gà
;WITH MonGaCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 4)
UPDATE m SET HinhAnh = '~/Content/imagesKFC_65133696/MonGa/MonGa' + CAST(c.STT AS NVARCHAR) + '.jpg'
FROM Mon_An m JOIN MonGaCTE c ON m.MaMon = c.MaMon;

-- Cơm Burger
;WITH CMBCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 5)
UPDATE m SET HinhAnh = '~/Content/imagesKFC_65133696/ComMiBurger/ComMiBurger' + CAST(c.STT AS NVARCHAR) + '.jpg'
FROM Mon_An m JOIN CMBCTE c ON m.MaMon = c.MaMon;

-- Giải khát
;WITH GiaiKhatCTE AS (SELECT MaMon, ROW_NUMBER() OVER (ORDER BY MaMon) AS STT FROM Mon_An WHERE MaLoai = 6)
UPDATE m SET HinhAnh = '~/Content/imagesKFC_65133696/GiaiKhat/GiaiKhat' + CAST(c.STT AS NVARCHAR) + '.jpg'
FROM Mon_An m JOIN GiaiKhatCTE c ON m.MaMon = c.MaMon;
GO

-- Trigger 1: Cập nhật tổng tiền đơn hàng
CREATE TRIGGER trg_CapNhatTongTienDon
ON Chi_Tiet_Don_Hang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @MaDH INT;
  SELECT TOP 1 @MaDH = MaDH FROM inserted;
  IF @MaDH IS NULL SELECT TOP 1 @MaDH = MaDH FROM deleted;
  UPDATE Don_Hang
  SET TongTien = ISNULL((SELECT SUM(SoLuong * DonGia) FROM Chi_Tiet_Don_Hang WHERE MaDH = @MaDH), 0)
  WHERE MaDH = @MaDH;
END
GO

-- Trigger 2: Tự động tạo Shipper khi DangGiao
CREATE TRIGGER trg_AutoTaoGiaoHang
ON Don_Hang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(TrangThai)
    BEGIN
        INSERT INTO Giao_Hang (MaDH, MaTK, TrangThai)
        SELECT d.MaDH, NULL, N'DangLayHang'
        FROM inserted d
        WHERE d.TrangThai = N'DangGiao' AND NOT EXISTS (SELECT 1 FROM Giao_Hang WHERE MaDH = d.MaDH);
    END
END
GO

-- Procedure 1: Đăng ký
CREATE PROCEDURE sp_DangKy
    @HoTen NVARCHAR(150), @Email NVARCHAR(150), @MatKhau NVARCHAR(100), @DiaChi NVARCHAR(255), @SoDT NVARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Tai_Khoan WHERE Email = @Email)
        SELECT -1 AS Result;
    ELSE
    BEGIN
        INSERT INTO Tai_Khoan (HoTen, Email, MatKhau, DiaChi, SoDT, VaiTro, TrangThai)
        VALUES (@HoTen, @Email, @MatKhau, @DiaChi, @SoDT, 'NguoiDung', 1);
        SELECT 1 AS Result;
    END
END
GO

-- Procedure 2: Đăng nhập
CREATE PROCEDURE sp_DangNhap
    @Email NVARCHAR(150), @MatKhau NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Tai_Khoan WHERE Email = @Email AND MatKhau = @MatKhau AND TrangThai = 1;
END
GO

-- Procedure 3: Tạo đơn hàng
CREATE PROCEDURE sp_TaoDonHang
    @MaTK INT, @DiaChiGiao NVARCHAR(255), @GhiChu NVARCHAR(255), @MaDH_OUT INT OUTPUT
AS
BEGIN
    INSERT INTO Don_Hang (MaTK, TongTien, TrangThai, DiaChiGiao, GhiChu)
    VALUES (@MaTK, 0, N'ChoXacNhan', @DiaChiGiao, @GhiChu);
    SET @MaDH_OUT = SCOPE_IDENTITY();
END
GO

-- Procedure 4: Thêm chi tiết đơn
CREATE PROCEDURE sp_ThemChiTietDon
    @MaDH INT, @MaMon INT, @SoLuong INT
AS
BEGIN
    DECLARE @DonGia DECIMAL(18,0);
    SELECT @DonGia = Gia FROM Mon_An WHERE MaMon = @MaMon;
    IF EXISTS (SELECT 1 FROM Chi_Tiet_Don_Hang WHERE MaDH=@MaDH AND MaMon=@MaMon)
        UPDATE Chi_Tiet_Don_Hang SET SoLuong = SoLuong + @SoLuong WHERE MaDH=@MaDH AND MaMon=@MaMon;
    ELSE
        INSERT INTO Chi_Tiet_Don_Hang (MaDH, MaMon, SoLuong, DonGia) VALUES (@MaDH, @MaMon, @SoLuong, @DonGia);
END
GO

-- Procedure 5: Gửi liên hệ
CREATE PROCEDURE sp_GuiLienHe
    @MaTK INT, @HoTen NVARCHAR(100), @Email VARCHAR(100), @NoiDung NTEXT
AS
BEGIN
    INSERT INTO LienHe (MaTK, HoTen, Email, NoiDung, NgayGui, TrangThai)
    VALUES (@MaTK, @HoTen, @Email, @NoiDung, GETDATE(), 0);
END
GO

-- Procedure 6: Thêm món (Admin)
CREATE PROCEDURE sp_ThemMon
    @MaLoai INT, @TenMon NVARCHAR(150), @MoTa NVARCHAR(MAX), @Gia DECIMAL(18,0), @HinhAnh NVARCHAR(255)
AS
BEGIN
    INSERT INTO Mon_An (MaLoai, TenMon, MoTa, Gia, HinhAnh, TrangThai)
    VALUES (@MaLoai, @TenMon, @MoTa, @Gia, @HinhAnh, 1);
END
GO

-- Procedure 7: Xóa món (Admin)
CREATE PROCEDURE sp_XoaMon @MaMon INT AS BEGIN UPDATE Mon_An SET TrangThai = 0 WHERE MaMon = @MaMon; END
GO

-- Procedure 8: Cập nhật giá (Admin)
CREATE PROCEDURE sp_CapNhatGiaMon @MaMon INT, @GiaMoi DECIMAL(18,0) AS BEGIN UPDATE Mon_An SET Gia = @GiaMoi WHERE MaMon = @MaMon; END
GO

-- Procedure 9: Thống kê doanh thu ngày
CREATE PROCEDURE sp_DoanhThuHomNay AS 
BEGIN 
    SELECT ISNULL(SUM(TongTien), 0) AS DoanhThu FROM Don_Hang WHERE CAST(NgayDat AS DATE) = CAST(GETDATE() AS DATE) AND TrangThai != N'Huy'; 
END
GO

-- Procedure 10: Tìm kiếm
CREATE PROCEDURE sp_TimKiemMon
    @Keyword NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Mon_An 
    WHERE TenMon LIKE N'%' + @Keyword + N'%' AND TrangThai = 1;
END
GO

-- =============================================
-- PHẦN 5: PHÂN QUYỀN
-- =============================================
CREATE ROLE db_Admin;
CREATE ROLE db_KhachHang;

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO db_Admin;
GRANT EXECUTE ON SCHEMA::dbo TO db_Admin;

GRANT SELECT ON Mon_An TO db_KhachHang;
GRANT SELECT ON Loai_Mon TO db_KhachHang;
GRANT SELECT ON Khuyen_Mai TO db_KhachHang;
GRANT SELECT ON Chi_Nhanh TO db_KhachHang;
GRANT INSERT, SELECT ON Don_Hang TO db_KhachHang;
GRANT INSERT ON Chi_Tiet_Don_Hang TO db_KhachHang;
GRANT INSERT ON LienHe TO db_KhachHang;
GRANT EXECUTE ON sp_DangKy TO db_KhachHang;
GRANT EXECUTE ON sp_DangNhap TO db_KhachHang;
GRANT EXECUTE ON sp_TaoDonHang TO db_KhachHang;
GRANT EXECUTE ON sp_ThemChiTietDon TO db_KhachHang;
GRANT EXECUTE ON sp_GuiLienHe TO db_KhachHang;
GRANT EXECUTE ON sp_TimKiemMon TO db_KhachHang;
GO