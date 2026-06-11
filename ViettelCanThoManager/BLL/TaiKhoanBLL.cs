using DAL;

namespace BLL
{
    public class TaiKhoanBLL
    {
        private TaiKhoanDAL tkDAL = new TaiKhoanDAL();

        public bool KiemTraDangNhap(string username, string password, out string errorMsg)
        {
            errorMsg = "";
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMsg = "Tên đăng nhập và mật khẩu không được để trống!";
                return false;
            }

            bool isSuccess = tkDAL.CheckLogin(username, password);
            if (!isSuccess)
            {
                errorMsg = "Sai tên đăng nhập hoặc mật khẩu!";
            }
            return isSuccess;
        }
    }
}