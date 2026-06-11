using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    // Kế thừa lại DBConnect để dùng chung biến kết nối 'conn'
    public class KhachHangDAL : DBConnect
    {
        public bool InsertKhachHang(string hoTen, string soDienThoai, string nhuCau, string diaChi)
        {
            string query = "INSERT INTO KhachHang (HoTen, SoDienThoai, NhuCau, DiaChi, ThoiGianDangKy, TrangThai) " +
                           "VALUES (@HoTen, @SoDienThoai, @NhuCau, @DiaChi, @ThoiGian, 0)";

            try
            {
                OpenConnection();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Sử dụng Parameter để bảo mật dữ liệu đầu vào
                    cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar).Value = hoTen;
                    cmd.Parameters.Add("@SoDienThoai", SqlDbType.VarChar).Value = soDienThoai;
                    cmd.Parameters.Add("@NhuCau", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(nhuCau) ? (object)DBNull.Value : nhuCau;
                    cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(diaChi) ? (object)DBNull.Value : diaChi;
                    cmd.Parameters.Add("@ThoiGian", SqlDbType.DateTime).Value = DateTime.Now;

                    int result = cmd.ExecuteNonQuery();
                    return result > 0; // Trả về true nếu thêm thành công dòng dữ liệu
                }
            }
            catch (Exception ex)
            {
                // Thực tế có thể log lỗi ex ở đây
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        // 1. Cập nhật hàm lấy danh sách (Dịch số thành chữ)
        public DataTable GetDanhSachKhachHang()
        {
            DataTable dt = new DataTable();
            // Dùng CASE WHEN để dịch trạng thái ngay từ SQL Server
            string query = @"SELECT Id, HoTen, SoDienThoai, NhuCau, DiaChi, ThoiGianDangKy, 
                     CASE TrangThai 
                        WHEN 0 THEN N'⏳ Chờ xử lý' 
                        WHEN 1 THEN N'📞 Đang khảo sát' 
                        WHEN 2 THEN N'✅ Hoàn thành' 
                        WHEN 3 THEN N'❌ Đã hủy' 
                     END AS TenTrangThai 
                     FROM KhachHang ORDER BY ThoiGianDangKy DESC";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception) { }
            finally { CloseConnection(); }
            return dt;
        }

        // 2. Viết thêm hàm Cập nhật trạng thái
        public bool UpdateTrangThai(int idKhachHang, int trangThaiMoi)
        {
            string query = "UPDATE KhachHang SET TrangThai = @TrangThai WHERE Id = @Id";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.Int).Value = trangThaiMoi;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idKhachHang;

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception) { return false; }
            finally { CloseConnection(); }
        }
        public DataTable GetThongKeTrangThai()
        {
            DataTable dt = new DataTable();
            // Dùng COUNT và GROUP BY để đếm số lượng mỗi trạng thái
            string query = "SELECT TrangThai, COUNT(Id) as SoLuong FROM KhachHang GROUP BY TrangThai";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception) { }
            finally { CloseConnection(); }
            return dt;
        }
    }
}