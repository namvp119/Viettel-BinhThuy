using System.Data;
using DAL;

namespace BLL
{
    public class GoiCuocBLL
    {
        private GoiCuocDAL gcDAL = new GoiCuocDAL();

        public DataTable LayDanhSachGoiCuoc()
        {
            return gcDAL.GetDanhSachGoiCuoc();
        }
        public bool ThemGoiCuoc(string tenGoi, string loaiDichVu, decimal giaTien, string moTa, out string errorMsg)
        {
            errorMsg = "";
            if (string.IsNullOrWhiteSpace(tenGoi))
            {
                errorMsg = "Tên gói cước không được để trống!";
                return false;
            }
            if (giaTien < 0)
            {
                errorMsg = "Giá tiền không hợp lệ!";
                return false;
            }

            bool isSuccess = gcDAL.InsertGoiCuoc(tenGoi, loaiDichVu, giaTien, moTa);
            if (!isSuccess) errorMsg = "Lỗi khi thêm vào cơ sở dữ liệu.";
            return isSuccess;
        }

        public bool SuaGoiCuoc(int id, string tenGoi, string loaiDichVu, decimal giaTien, string moTa, out string errorMsg)
        {
            errorMsg = "";
            if (string.IsNullOrWhiteSpace(tenGoi))
            {
                errorMsg = "Tên gói cước không được để trống!";
                return false;
            }

            bool isSuccess = gcDAL.UpdateGoiCuoc(id, tenGoi, loaiDichVu, giaTien, moTa);
            if (!isSuccess) errorMsg = "Lỗi khi cập nhật cơ sở dữ liệu.";
            return isSuccess;
        }

        public bool XoaGoiCuoc(int id)
        {
            return gcDAL.DeleteGoiCuoc(id);
        }
    }   

}