using System;
using System.Data.SqlClient;

namespace DAL
{
    public class TaiKhoanDAL : DBConnect
    {
        public bool CheckLogin(string username, string password)
        {
            string query = "SELECT COUNT(1) FROM TaiKhoan WHERE TenDangNhap = @User AND MatKhau = @Pass";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Pass", password);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Trả về true nếu tài khoản tồn tại
                }
            }
            catch (Exception) { return false; }
            finally { CloseConnection(); }
        }
    }
}