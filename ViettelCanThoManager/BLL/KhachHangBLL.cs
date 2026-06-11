using System;
using DAL;
using System.Data;
namespace BLL
{
    public class KhachHangBLL
    {
        private KhachHangDAL khDAL = new KhachHangDAL();

        public bool DangKyTuVan(string hoTen, string soDienThoai, string nhuCau, string diaChi, out string errorMsg)
        {
            errorMsg = "";

            // 1. Kiểm tra Validate logic nghiệp vụ
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                errorMsg = "Họ và tên không được để trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(soDienThoai) || soDienThoai.Length < 10 || soDienThoai.Length > 11)
            {
                errorMsg = "Số điện thoại không hợp lệ (Phải từ 10 - 11 số).";
                return false;
            }

            // 2. Nếu mọi thứ hợp lệ, gọi xuống lớp DAL để lưu vào SQL Server
            bool isSuccess = khDAL.InsertKhachHang(hoTen, soDienThoai, nhuCau, diaChi);
            if (!isSuccess)
            {
                errorMsg = "Lỗi hệ thống! Không thể lưu dữ liệu vào Cơ sở dữ liệu.";
            }

            return isSuccess;
        }
        public DataTable LayDanhSachKhachHang()
        {
            // Ở đây BLL có thể xử lý thêm logic lọc dữ liệu nếu cần, 
            // tạm thời chúng ta cứ lấy lên hết.
            return khDAL.GetDanhSachKhachHang();
        }
        public bool CapNhatTrangThaiKhach(int id, int trangThaiMoi)
        {
            return khDAL.UpdateTrangThai(id, trangThaiMoi);
        }
        public DataTable LayThongKeTrangThai()
        {
            return khDAL.GetThongKeTrangThai();
        }
    }
}