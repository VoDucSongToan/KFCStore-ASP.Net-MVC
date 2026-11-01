# KFC Shop – ASP.NET MVC 5

Đồ án môn Phát triển ứng dụng Web – Võ Đức Song Toàn (65133696).

## Công nghệ
- ASP.NET MVC 5, C#, Razor Views (.NET Framework 4.7.2)
- Entity Framework 6 (Database First), LINQ
- SQL Server, Bootstrap 5, jQuery
- Visual Studio 2022

## Chức năng
- **Khách hàng:** đăng ký, đăng nhập, xem thực đơn, lọc món, giỏ hàng, đặt hàng, xem lịch sử đơn hàng, liên hệ và khuyến mãi.
- **Quản trị:** quản lý món ăn, loại món, đơn hàng, khách hàng, khuyến mãi, phản hồi, thống kê doanh thu.

## Trạng thái
**Mã nguồn đầy đủ chưa được tải lên repository này.** Bộ mã nguồn đã được chuẩn bị trong cuộc trò chuyện ChatGPT dưới dạng tệp ZIP `CNPM_KFC_65133696_san_sang_push.zip`. Repo hiện được chuẩn bị phần mô tả; không coi README này là xác nhận đã tải lên/kiểm thử ứng dụng.

## Hướng dẫn chạy sau khi thêm mã nguồn
1. Mở file `QuanLyGaRanKFC_65133696/QuanLyGaRanKFC_65133696.sln` bằng Visual Studio 2022.
2. Chạy script `65133696_VoDucSongToan_QuanLyKFC_CuoiKy.sql` trong SQL Server Management Studio để tạo CSDL `KFC_Store`.
3. Sửa SQL Server name trong `Web.config` (hai connection strings), restore NuGet packages, rồi nhấn F5.
4. Vào trang `/KhachHang_65133696/TrangChu_65133696`.

> Đây là dự án học tập, không phải website chính thức của KFC. Không dùng tài khoản mẫu và mật khẩu mẫu trong môi trường thật.
