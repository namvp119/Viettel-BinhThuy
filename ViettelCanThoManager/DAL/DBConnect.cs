using System;
using System.Data;
using System.Data.SqlClient; // Thư viện cốt lõi để gọi SQL Server

namespace DAL
{
    public class DBConnect
    {
        // Chuỗi kết nối đến Database ViettelCanThoDB
        // LƯU Ý: Phải thay "TEN_MAY_TINH\SQLEXPRESS" bằng tên Server của bạn
        private string connString = @"Data Source=DESKTOP-C1FMF3R\SQLEXPRESS;Initial Catalog=ViettelCanThoDB;Integrated Security=True";

        protected SqlConnection conn;

        public DBConnect()
        {
            conn = new SqlConnection(connString);
        }

        // Hàm mở kết nối
        public void OpenConnection()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        // Hàm đóng kết nối
        public void CloseConnection()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}