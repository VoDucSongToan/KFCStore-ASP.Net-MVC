using QuanLyGaRanKFC_65133696.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyGaRanKFC_65133696.Controllers
{
    public class QuanLy_65133696Controller : Controller
    {
        private KFC_StoreEntities db = new KFC_StoreEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = Session["NguoiDung_65133696"] as Tai_Khoan;
            if (user == null || user.VaiTro != "QuanLy")
            {
                filterContext.Result = RedirectToAction("DangNhap_65133696", "KhachHang_65133696");
            }
            base.OnActionExecuting(filterContext);
        }

        // =========================================================
        // 1. TRANG TỔNG QUAN (DASHBOARD)
        // =========================================================
        public ActionResult TrangChuAdmin_65133696()
        {
            ViewBag.TongDoanhThu = db.Don_Hang.Where(d => d.TrangThai == "HoanThanh").Sum(d => d.TongTien) ?? 0;
            ViewBag.TongDonHang = db.Don_Hang.Count();
            ViewBag.DonChoXacNhan = db.Don_Hang.Count(d => d.TrangThai == "ChoXacNhan");
            ViewBag.TongKhachHang = db.Tai_Khoan.Count(u => u.VaiTro == "NguoiDung");
            return View();
        }

        // =========================================================
        // 2. QUẢN LÝ ĐƠN HÀNG
        // =========================================================
        public ActionResult QuanLyDonHang_65133696()
        {
            var listDon = db.Don_Hang.OrderByDescending(d => d.NgayDat).ToList();
            return View(listDon);
        }

        public ActionResult ChiTietDonHangAdmin_65133696(int id)
        {
            var donHang = db.Don_Hang.Find(id);
            if (donHang == null) return HttpNotFound();

            var chiTiet = db.Chi_Tiet_Don_Hang.Include(c => c.Mon_An).Where(c => c.MaDH == id).ToList();
            ViewBag.DonHang = donHang;
            return View(chiTiet);
        }

        public ActionResult DuyetDon_65133696(int id)
        {
            var dh = db.Don_Hang.Find(id);
            if (dh != null)
            {
                dh.TrangThai = "DangGiao";
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyDonHang_65133696");
        }

        public ActionResult HoanThanhDon_65133696(int id)
        {
            var dh = db.Don_Hang.Find(id);
            if (dh != null)
            {
                dh.TrangThai = "HoanThanh";
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyDonHang_65133696");
        }

        public ActionResult HuyDon_65133696(int id)
        {
            var dh = db.Don_Hang.Find(id);
            if (dh != null)
            {
                dh.TrangThai = "Huy";
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyDonHang_65133696");
        }

        // =========================================================
        // 3. QUẢN LÝ LOẠI MÓN (DANH MỤC)
        // =========================================================
        public ActionResult QuanLyLoaiMon_65133696()
        {
            var listLoai = db.Loai_Mon.ToList();
            return View(listLoai);
        }

        [HttpPost]
        public ActionResult ThemLoaiMon_65133696(string TenLoai)
        {
            if (!string.IsNullOrEmpty(TenLoai))
            {
                var loai = new Loai_Mon();
                loai.TenLoai = TenLoai;
                db.Loai_Mon.Add(loai);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm danh mục thành công!";
            }
            return RedirectToAction("QuanLyLoaiMon_65133696");
        }

        public ActionResult SuaLoaiMon_65133696(int id)
        {
            var loai = db.Loai_Mon.Find(id);
            if (loai == null) return HttpNotFound();
            return View(loai);
        }

        [HttpPost]
        public ActionResult SuaLoaiMon_65133696(Loai_Mon model)
        {
            if (ModelState.IsValid)
            {
                var loai = db.Loai_Mon.Find(model.MaLoai);
                if (loai != null)
                {
                    loai.TenLoai = model.TenLoai;
                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("QuanLyLoaiMon_65133696");
                }
            }
            return View(model);
        }

        public ActionResult XoaLoaiMon_65133696(int id)
        {
            var loai = db.Loai_Mon.Find(id);
            if (loai != null)
            {
                bool dangSuDung = db.Mon_An.Any(m => m.MaLoai == id);
                if (dangSuDung)
                {
                    TempData["Loi"] = "Không thể xóa! Đang có món ăn thuộc danh mục này.";
                }
                else
                {
                    db.Loai_Mon.Remove(loai);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Đã xóa danh mục.";
                }
            }
            return RedirectToAction("QuanLyLoaiMon_65133696");
        }

        // =========================================================
        // 4. QUẢN LÝ MÓN ĂN
        // =========================================================
        public ActionResult QuanLyMonAn_65133696()
        {
            var listMon = db.Mon_An.Include(m => m.Loai_Mon).OrderByDescending(m => m.MaMon).ToList();
            return View(listMon);
        }

        public ActionResult ThemMonAn_65133696()
        {
            ViewBag.MaLoai = new SelectList(db.Loai_Mon, "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMonAn_65133696(Mon_An model, HttpPostedFileBase fileAnh)
        {
            if (ModelState.IsValid)
            {
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileAnh.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/imagesKFC_65133696/MonMoi"), _FileName);
                    fileAnh.SaveAs(_path);
                    model.HinhAnh = "~/Content/imagesKFC_65133696/MonMoi/" + _FileName;
                }
                else
                {
                    model.HinhAnh = "~/Content/imagesKFC_65133696/MonMoi/default_food.jpg";
                }

                model.TrangThai = true;
                db.Mon_An.Add(model);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm món ăn mới thành công!";
                return RedirectToAction("QuanLyMonAn_65133696");
            }
            ViewBag.MaLoai = new SelectList(db.Loai_Mon, "MaLoai", "TenLoai", model.MaLoai);
            return View(model);
        }

        public ActionResult SuaMonAn_65133696(int id)
        {
            var mon = db.Mon_An.Find(id);
            if (mon == null) return HttpNotFound();

            ViewBag.MaLoai = new SelectList(db.Loai_Mon, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaMonAn_65133696(Mon_An model, HttpPostedFileBase fileAnh)
        {
            if (ModelState.IsValid)
            {
                var monCu = db.Mon_An.Find(model.MaMon);
                if (monCu != null)
                {
                    monCu.TenMon = model.TenMon;
                    monCu.Gia = model.Gia;
                    monCu.MoTa = model.MoTa;
                    monCu.MaLoai = model.MaLoai;
                    monCu.TrangThai = model.TrangThai;

                    if (fileAnh != null && fileAnh.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(fileAnh.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/imagesKFC_65133696/MonMoi"), _FileName);
                        fileAnh.SaveAs(_path);
                        monCu.HinhAnh = "~/Content/imagesKFC_65133696/MonMoi/" + _FileName;
                    }

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật món ăn thành công!";
                    return RedirectToAction("QuanLyMonAn_65133696");
                }
            }
            ViewBag.MaLoai = new SelectList(db.Loai_Mon, "MaLoai", "TenLoai", model.MaLoai);
            return View(model);
        }

        public ActionResult XoaMonAn_65133696(int id)
        {
            var mon = db.Mon_An.Find(id);
            if (mon != null)
            {
                mon.TrangThai = false;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã ngừng kinh doanh món này.";
            }
            return RedirectToAction("QuanLyMonAn_65133696");
        }

        // =========================================================
        // 5. QUẢN LÝ KHÁCH HÀNG
        // =========================================================
        public ActionResult QuanLyKhachHang_65133696()
        {
            var list = db.Tai_Khoan.OrderByDescending(u => u.VaiTro)
                                   .ThenByDescending(u => u.MaTK)
                                   .ToList();
            return View(list);
        }

        public ActionResult ThemKhachHang_65133696()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemKhachHang_65133696(Tai_Khoan model)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = db.Tai_Khoan.FirstOrDefault(x => x.Email == model.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại!");
                    return View(model);
                }

                model.VaiTro = "NguoiDung";
                db.Tai_Khoan.Add(model);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm thành công!";
                return RedirectToAction("QuanLyKhachHang_65133696");
            }
            return View(model);
        }

        public ActionResult SuaKhachHang_65133696(int id)
        {
            var kh = db.Tai_Khoan.Find(id);
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        [HttpPost]
        public ActionResult SuaKhachHang_65133696(Tai_Khoan model)
        {
            if (ModelState.IsValid)
            {
                var kh = db.Tai_Khoan.Find(model.MaTK);
                if (kh != null)
                {
                    kh.HoTen = model.HoTen;
                    kh.SoDT = model.SoDT;
                    kh.DiaChi = model.DiaChi;

                    if (!string.IsNullOrEmpty(model.MatKhau))
                    {
                        kh.MatKhau = model.MatKhau;
                    }

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật thành công!";
                    return RedirectToAction("QuanLyKhachHang_65133696");
                }
            }
            return View(model);
        }

        public ActionResult XoaKhachHang_65133696(int id)
        {
            var kh = db.Tai_Khoan.Find(id);
            if (kh != null)
            {
                try
                {
                    db.Tai_Khoan.Remove(kh);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Đã xóa tài khoản.";
                }
                catch (Exception)
                {
                    TempData["Loi"] = "Không thể xóa! Tài khoản này đang chứa dữ liệu quan trọng.";
                }
            }
            return RedirectToAction("QuanLyKhachHang_65133696");
        }

        public ActionResult CapQuyenAdmin_65133696(int id)
        {
            var kh = db.Tai_Khoan.Find(id);
            if (kh != null)
            {
                kh.VaiTro = "QuanLy";
                db.SaveChanges();
                TempData["ThongBao"] = "Đã thăng chức QUẢN TRỊ VIÊN cho: " + kh.HoTen;
            }
            return RedirectToAction("QuanLyKhachHang_65133696");
        }

        public ActionResult HuyQuyenAdmin_65133696(int id)
        {
            var kh = db.Tai_Khoan.Find(id);
            if (kh != null)
            {
                var adminDangNhap = Session["NguoiDung_65133696"] as Tai_Khoan;
                if (adminDangNhap != null && adminDangNhap.MaTK == id)
                {
                    TempData["Loi"] = "Bạn không thể tự hủy quyền Admin của chính mình!";
                }
                else
                {
                    kh.VaiTro = "NguoiDung";
                    db.SaveChanges();
                    TempData["ThongBao"] = "Đã hủy quyền Admin của tài khoản: " + kh.HoTen;
                }
            }
            return RedirectToAction("QuanLyKhachHang_65133696");
        }

        // =========================================================
        // 6. QUẢN LÝ KHUYẾN MÃI (VOUCHER)
        // =========================================================
        public ActionResult QuanLyKhuyenMai_65133696()
        {
            var listKM = db.Khuyen_Mai.OrderByDescending(k => k.NgayBatDau).ToList();
            return View(listKM);
        }

        public ActionResult ThemKhuyenMai_65133696()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemKhuyenMai_65133696(Khuyen_Mai model)
        {
            if (ModelState.IsValid)
            {
                model.TrangThai = true;
                db.Khuyen_Mai.Add(model);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã tạo Voucher khuyến mãi mới!";
                return RedirectToAction("QuanLyKhuyenMai_65133696");
            }
            return View(model);
        }

        public ActionResult SuaKhuyenMai_65133696(int id)
        {
            var km = db.Khuyen_Mai.Find(id);
            if (km == null) return HttpNotFound();
            return View(km);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaKhuyenMai_65133696(Khuyen_Mai model)
        {
            if (ModelState.IsValid)
            {
                var km = db.Khuyen_Mai.Find(model.MaKM);
                if (km != null)
                {
                    km.TenKM = model.TenKM;
                    km.PhanTramGiam = model.PhanTramGiam;
                    km.NgayBatDau = model.NgayBatDau;
                    km.NgayKetThuc = model.NgayKetThuc;
                    km.NoiDung = model.NoiDung;

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật Voucher thành công!";
                    return RedirectToAction("QuanLyKhuyenMai_65133696");
                }
            }
            return View(model);
        }

        public ActionResult XoaKhuyenMai_65133696(int id)
        {
            var km = db.Khuyen_Mai.Find(id);
            if (km != null)
            {
                db.Khuyen_Mai.Remove(km);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã xóa Voucher.";
            }
            return RedirectToAction("QuanLyKhuyenMai_65133696");
        }

        // =========================================================
        // 7. QUẢN LÝ PHẢN HỒI KHÁCH HÀNG
        // =========================================================
        public ActionResult QuanLyPhanHoi_65133696()
        {
            var listPH = db.LienHes.OrderByDescending(x => x.NgayGui).ToList();
            return View(listPH);
        }

        [HttpPost]
        public ActionResult TraLoiPhanHoi_65133696(int id, string noiDungTraLoi)
        {
            var lh = db.LienHes.Find(id);
            if (lh != null)
            {
                lh.PhanHoi = noiDungTraLoi;
                lh.TrangThai = true;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã gửi phản hồi thành công!";
            }
            return RedirectToAction("QuanLyPhanHoi_65133696");
        }

        // =========================================================
        // 8. BÁO CÁO DOANH THU
        // =========================================================
        public ActionResult ThongKeDoanhThu_65133696()
        {
            var danhSachDonHoanThanh = db.Don_Hang
                                         .Include("Tai_Khoan")
                                         .Where(d => d.TrangThai == "HoanThanh")
                                         .OrderByDescending(d => d.NgayDat)
                                         .ToList();
            return View(danhSachDonHoanThanh);
        }
    }
}