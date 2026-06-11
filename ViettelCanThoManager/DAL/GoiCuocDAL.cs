using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class GoiCuocDAL : DBConnect
    {
        public DataTable GetDanhSachGoiCuoc()
        {
            DataTable dt = new DataTable();
            // Lấy danh sách gói cước đang được phép hiển thị
            string query = "SELECT Id, TenGoi, LoaiDichVu, GiaTien, MoTa FROM GoiCuoc WHERE TrangThaiHienThi = 1";

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
            catch (Exception ex)
            {
                // Thực tế nên log lỗi
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }
        // 1. Hàm Thêm gói cước mới
        public bool InsertGoiCuoc(string tenGoi, string loaiDichVu, decimal giaTien, string moTa)
        {
            string query = "INSERT INTO GoiCuoc (TenGoi, LoaiDichVu, GiaTien, MoTa, TrangThaiHienThi) " +
                           "VALUES (@TenGoi, @LoaiDichVu, @GiaTien, @MoTa, 1)";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@TenGoi", SqlDbType.NVarChar).Value = tenGoi;
                    cmd.Parameters.Add("@LoaiDichVu", SqlDbType.NVarChar).Value = loaiDichVu;
                    cmd.Parameters.Add("@GiaTien", SqlDbType.Decimal).Value = giaTien;
                    cmd.Parameters.Add("@MoTa", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(moTa) ? (object)DBNull.Value : moTa;
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
            finally { CloseConnection(); }
        }

        // 2. Hàm Sửa gói cước
        public bool UpdateGoiCuoc(int id, string tenGoi, string loaiDichVu, decimal giaTien, string moTa)
        {
            string query = "UPDATE GoiCuoc SET TenGoi = @TenGoi, LoaiDichVu = @LoaiDichVu, " +
                           "GiaTien = @GiaTien, MoTa = @MoTa WHERE Id = @Id";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@TenGoi", SqlDbType.NVarChar).Value = tenGoi;
                    cmd.Parameters.Add("@LoaiDichVu", SqlDbType.NVarChar).Value = loaiDichVu;
                    cmd.Parameters.Add("@GiaTien", SqlDbType.Decimal).Value = giaTien;
                    cmd.Parameters.Add("@MoTa", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(moTa) ? (object)DBNull.Value : moTa;
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
            finally { CloseConnection(); }
        }

        // 3. Hàm Xóa (Ẩn) gói cước
        public bool DeleteGoiCuoc(int id)
        {
            // Soft Delete: Chuyển TrangThaiHienThi = 0 thay vì xóa hẳn
            string query = "UPDATE GoiCuoc SET TrangThaiHienThi = 0 WHERE Id = @Id";
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
            finally { CloseConnection(); }
        }
    }
}