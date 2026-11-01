using QuanLyGaRanKFC_65133696.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace QuanLyGaRanKFC_65133696.Controllers
{
    public class KhachHang_65133696Controller : Controller
    {
        private KFC_StoreEntities db = new KFC_StoreEntities();

        // 1. TRANG CHỦ
        public ActionResult TrangChu_65133696()
        {
            var listTop4 = db.Mon_An
                             .Include("Loai_Mon")
                             .Where(m => m.TrangThai == true)
                             .OrderByDescending(m => m.Gia)
                             .Take(4)
                             .ToList();
            return View(listTop4);
        }

        // 2. ĐĂNG NHẬP
        public ActionResult DangNhap_65133696()
        {
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap_65133696(string Email, string MatKhau)
        {
            var tk = db.Tai_Khoan.FirstOrDefault(x => x.Email == Email && x.MatKhau == MatKhau);
            if (tk != null)
            {
                Session["NguoiDung_65133696"] = tk;
                Session["VaiTro_65133696"] = tk.VaiTro;

                if (tk.VaiTro == "QuanLy")
                {
                    return RedirectToAction("TrangChuAdmin_65133696", "QuanLy_65133696");
                }
                else
                {
                    return RedirectToAction("TrangChu_65133696");
                }
            }
            ViewBag.Loi = "Sai thông tin đăng nhập hoặc tài khoản không tồn tại.";
            return View();
        }

        // 3. ĐĂNG KÝ
        public ActionResult DangKy_65133696() { return View(); }

        [HttpPost]
        public ActionResult DangKy_65133696(Tai_Khoan model, string xacNhanMatKhau)
        {
            if (ModelState.IsValid)
            {
                if (model.MatKhau != xacNhanMatKhau)
                {
                    ModelState.AddModelError("", "Mật khẩu xác nhận không trùng khớp!");
                    return View(model);
                }

                var checkEmail = db.Tai_Khoan.FirstOrDefault(x => x.Email == model.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("", "Email này đã được đăng ký.");
                    return View(model);
                }

                model.VaiTro = "NguoiDung";
                db.Tai_Khoan.Add(model);
                db.SaveChanges();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap_65133696");
            }
            return View(model);
        }

        // 4. ĐĂNG XUẤT
        public ActionResult DangXuat_65133696()
        {
            Session.Clear();
            return RedirectToAction("TrangChu_65133696");
        }

        // 5. THÔNG TIN CÁ NHÂN & CẬP NHẬT
        public ActionResult ThongTinCaNhan_65133696()
        {
            var userSession = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (userSession == null) return RedirectToAction("DangNhap_65133696");

            var khachHang = db.Tai_Khoan.Find(userSession.MaTK);
            if (khachHang == null) return HttpNotFound();

            ViewBag.ListDonHang = db.Don_Hang
                                    .Where(d => d.MaTK == khachHang.MaTK)
                                    .OrderByDescending(d => d.NgayDat)
                                    .ToList();

            ViewBag.ListPhanHoi = db.LienHes
                                    .Where(l => l.MaTK == khachHang.MaTK)
                                    .OrderByDescending(l => l.NgayGui)
                                    .ToList();

            return View(khachHang);
        }

        [HttpPost]
        public ActionResult CapNhatThongTin_65133696(string hoTen, string soDienThoai, string diaChi)
        {
            var userSession = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (userSession == null) return RedirectToAction("DangNhap_65133696");

            var tk = db.Tai_Khoan.Find(userSession.MaTK);
            if (tk != null)
            {
                tk.HoTen = hoTen;
                tk.SoDT = soDienThoai;
                tk.DiaChi = diaChi;
                db.SaveChanges();

                Session["NguoiDung_65133696"] = tk;
                TempData["ThongBao"] = "Cập nhật thông tin thành công!";
            }
            return RedirectToAction("ThongTinCaNhan_65133696");
        }
        [HttpPost]
        public ActionResult DoiMatKhau_65133696(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            
            var userSession = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (userSession == null) return RedirectToAction("DangNhap_65133696");

            var tk = db.Tai_Khoan.Find(userSession.MaTK);

            if (tk != null)
            {
                if (tk.MatKhau != matKhauCu)
                {
                    TempData["LoiDoiMatKhau"] = "Mật khẩu hiện tại không chính xác!";
                    TempData["OpenTab"] = "tabPassword";
                    return RedirectToAction("ThongTinCaNhan_65133696");
                }

                if (matKhauMoi != xacNhanMatKhau)
                {
                    TempData["LoiDoiMatKhau"] = "Mật khẩu xác nhận không trùng khớp!";
                    TempData["OpenTab"] = "tabPassword";
                    return RedirectToAction("ThongTinCaNhan_65133696");
                }

                tk.MatKhau = matKhauMoi;
                db.SaveChanges();

                Session["NguoiDung_65133696"] = tk;
                TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            }

            return RedirectToAction("ThongTinCaNhan_65133696");
        }
        // 6. THỰC ĐƠN & SẮP XẾP
        public ActionResult DanhSachMon_65133696(int? maLoai, string sortOrder)
        {
            var listLoai = db.Loai_Mon.ToList();
            var query = db.Mon_An.Where(m => m.TrangThai == true);

            if (maLoai.HasValue && maLoai.Value > 0)
            {
                query = query.Where(m => m.MaLoai == maLoai.Value);
            }
            else
            {
                maLoai = 0;
            }

            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "price_asc": query = query.OrderBy(m => m.Gia); break;
                case "price_desc": query = query.OrderByDescending(m => m.Gia); break;
                case "name_asc": query = query.OrderBy(m => m.TenMon); break;
                case "newest":
                default: query = query.OrderByDescending(m => m.MaMon); break;
            }

            ViewBag.MaLoaiChon = maLoai.Value;
            ViewBag.CurrentMonAn = query.ToList();

            return View(listLoai);
        }

        // 7. CHI TIẾT MÓN
        public ActionResult ChiTietMon_65133696(int id)
        {
            var mon = db.Mon_An.Include("Loai_Mon").FirstOrDefault(m => m.MaMon == id);
            if (mon == null) return HttpNotFound();
            return View(mon);
        }

        // 8. QUẢN LÝ GIỎ HÀNG
        public ActionResult ThemVaoGio_65133696(int id, int soLuong = 1) 
        {
            var gioHang = Session["GioHang_65133696"] as Dictionary<int, int> ?? new Dictionary<int, int>();
            if (gioHang.ContainsKey(id))
            {
                gioHang[id] += soLuong; 
            }
            else
            {
                gioHang.Add(id, soLuong); 
            }

            Session["GioHang_65133696"] = gioHang;
            // Trả về trang cũ
            return Request.UrlReferrer != null ? (ActionResult)Redirect(Request.UrlReferrer.ToString()) : RedirectToAction("DanhSachMon_65133696");
        }
        public ActionResult GioHang_65133696(int? voucherId)
        {
            var gioHang = Session["GioHang_65133696"] as Dictionary<int, int>;
            if (gioHang == null || gioHang.Count == 0) return View(new List<Mon_An>());

            var listMaMon = gioHang.Keys.ToList();
            var listMonAn = db.Mon_An.Include("Loai_Mon").Where(m => listMaMon.Contains(m.MaMon)).ToList();

            var savedIds = Session["VoucherDaLuu_65133696"] as List<int> ?? new List<int>();
            var myVouchers = db.Khuyen_Mai.Where(k => savedIds.Contains(k.MaKM) && k.TrangThai == true).ToList();
            ViewBag.VouchersCuaToi = myVouchers;

            decimal giamGia = 0;
            if (voucherId.HasValue)
            {
                var vChon = myVouchers.FirstOrDefault(v => v.MaKM == voucherId);
                if (vChon != null)
                {
                    decimal tong = listMonAn.Sum(m => m.Gia * gioHang[m.MaMon]);
                    giamGia = tong * (decimal)vChon.PhanTramGiam / 100;
                    ViewBag.ThongBaoVoucher = $"Áp dụng: {vChon.TenKM} (-{vChon.PhanTramGiam}%)";
                }
            }

            ViewBag.GiamGia = giamGia;
            ViewBag.VoucherIdDangChon = voucherId;
            return View(listMonAn);
        }

        public ActionResult ThayDoiSoLuong_65133696(int id, string hanhDong)
        {
            var gioHang = Session["GioHang_65133696"] as Dictionary<int, int>;
            if (gioHang != null && gioHang.ContainsKey(id))
            {
                switch (hanhDong)
                {
                    case "tang":
                        gioHang[id]++;
                        break;
                    case "giam":
                        gioHang[id]--;
                        if (gioHang[id] <= 0) gioHang.Remove(id);
                        break;
                    case "xoa":
                        gioHang.Remove(id);
                        break;
                }
                Session["GioHang_65133696"] = gioHang;
            }
            return RedirectToAction("GioHang_65133696");
        }

        public ActionResult XoaTatCaGioHang_65133696()
        {
            Session["GioHang_65133696"] = null;
            return RedirectToAction("GioHang_65133696");
        }

        // 9. ĐẶT HÀNG
        [HttpPost]
        public ActionResult DatHang_65133696(string hoTenNguoiNhan, string soDienThoai, string diaChiGiao, string ghiChu)
        {
            var userSession = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (userSession == null) return RedirectToAction("DangNhap_65133696");

            var gioHang = Session["GioHang_65133696"] as Dictionary<int, int>;
            if (gioHang == null || gioHang.Count == 0) return RedirectToAction("TrangChu_65133696");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var donHang = new Don_Hang();
                    donHang.MaTK = userSession.MaTK;
                    donHang.NgayDat = DateTime.Now;
                    donHang.TrangThai = "ChoXacNhan";
                    donHang.DiaChiGiao = diaChiGiao;
                    donHang.GhiChu = $"{ghiChu} (Người nhận: {hoTenNguoiNhan} - SĐT: {soDienThoai})";

                    db.Don_Hang.Add(donHang);
                    db.SaveChanges();

                    decimal tongTien = 0;
                    foreach (var item in gioHang)
                    {
                        var monAn = db.Mon_An.Find(item.Key);
                        if (monAn != null)
                        {
                            var chiTiet = new Chi_Tiet_Don_Hang();
                            chiTiet.MaDH = donHang.MaDH;
                            chiTiet.MaMon = item.Key;
                            chiTiet.SoLuong = item.Value;
                            chiTiet.DonGia = monAn.Gia;

                            db.Chi_Tiet_Don_Hang.Add(chiTiet);
                            tongTien += (monAn.Gia * item.Value);
                        }
                    }

                    donHang.TongTien = tongTien;
                    db.SaveChanges();
                    transaction.Commit();

                    Session["GioHang_65133696"] = null;
                    TempData["ThongBao"] = "Đặt hàng thành công! Đơn hàng đang chờ xác nhận.";
                    return RedirectToAction("ThongTinCaNhan_65133696");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.Loi = "Lỗi đặt hàng: " + ex.Message;
                    return RedirectToAction("GioHang_65133696");
                }
            }
        }

        public ActionResult ChiTietDonHang_65133696(int id)
        {
            var user = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (user == null) return RedirectToAction("DangNhap_65133696");

            var donHang = db.Don_Hang.FirstOrDefault(d => d.MaDH == id && d.MaTK == user.MaTK);
            if (donHang == null) return RedirectToAction("ThongTinCaNhan_65133696");

            var chiTiet = db.Chi_Tiet_Don_Hang.Include("Mon_An")
                                              .Where(c => c.MaDH == id)
                                              .ToList();
            ViewBag.DonHang = donHang;
            return View(chiTiet);
        }

        // 10. KHUYẾN MÃI
        public ActionResult KhuyenMai_65133696()
        {
            var listVoucher = db.Khuyen_Mai.Where(x => x.TrangThai == true).OrderByDescending(x => x.PhanTramGiam).Take(6).ToList();
            ViewBag.MaDaLuu = Session["VoucherDaLuu_65133696"] as List<int> ?? new List<int>();
            ViewBag.DanhSachVoucher = listVoucher;

            var listMonCombo = db.Mon_An.Include("Loai_Mon")
                                     .Where(m => m.TrangThai == true && (m.Loai_Mon.TenLoai.Contains("Combo") || m.Loai_Mon.TenLoai.Contains("Ưu đãi")))
                                     .OrderBy(m => m.Gia).ToList();
            return View(listMonCombo);
        }

        public ActionResult LuuVoucher_65133696(int maKM)
        {
            var saved = Session["VoucherDaLuu_65133696"] as List<int> ?? new List<int>();
            if (!saved.Contains(maKM)) { saved.Add(maKM); Session["VoucherDaLuu_65133696"] = saved; }
            return RedirectToAction("KhuyenMai_65133696");
        }

        // 11. LIÊN HỆ & LỊCH SỬ
        [HttpGet]
        public ActionResult LienHe_65133696()
        {
            var model = new LienHe();
            var user = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (user != null)
            {
                model.MaTK = user.MaTK;
                model.HoTen = user.HoTen;
                model.Email = user.Email;
                model.SoDienThoai = user.SoDT;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult LienHe_65133696(LienHe model)
        {
            if (ModelState.IsValid)
            {
                model.NgayGui = DateTime.Now;
                model.TrangThai = false;
                db.LienHes.Add(model);
                db.SaveChanges();
                TempData["ThongBao"] = "Cảm ơn bạn đã liên hệ! KFC sẽ phản hồi sớm nhất.";
                return RedirectToAction("LienHe_65133696");
            }
            TempData["Loi"] = "Vui lòng nhập đầy đủ thông tin (*).";
            return View(model);
        }

        public ActionResult LichSuPhanHoi_65133696()
        {
            var user = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (user == null)
            {
                return RedirectToAction("DangNhap_65133696", "KhachHang_65133696");
            }

            var listLienHe = db.LienHes
                               .Where(x => x.MaTK == user.MaTK)
                               .OrderByDescending(x => x.NgayGui)
                               .ToList();
            return View(listLienHe);
        }

        // 12. HỆ THỐNG NHÀ HÀNG
        public ActionResult HeThongNhaHang_65133696()
        {
            var listCN = db.Chi_Nhanh.ToList();
            return View(listCN);
        }
    }
}