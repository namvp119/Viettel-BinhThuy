using System;
using System.Windows;
using System.Data;
using BLL; // Gọi BLL lên xài
using Microsoft.AspNetCore.SignalR.Client; // [THÊM MỚI] Thư viện để bắt sóng SignalR
using OfficeOpenXml; // Thư viện EPPlus
using Microsoft.Win32; // Để dùng hộp thoại SaveFileDialog
using System.IO; // Để thao tác với File
using LiveCharts;
using LiveCharts.Wpf;
namespace GUI

{
    public partial class MainWindow : Window
    {
        private KhachHangBLL khBLL = new KhachHangBLL();
        private GoiCuocBLL gcBLL = new GoiCuocBLL();
        // [THÊM MỚI] Khai báo biến kết nối
        private HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();

            // Chỉ giữ lại ĐÚNG 1 DÒNG NÀY thôi nhé, tuyệt đối không chèn thêm LicenseContext
            OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Do An Sinh Vien");
            LoadDataGoiCuoc();
            LoadData();
            ConnectSignalR();
        }
        private void LoadDataGoiCuoc()
        {
            dgvGoiCuoc.ItemsSource = gcBLL.LayDanhSachGoiCuoc().DefaultView;
        }
        // [THÊM MỚI] Hàm xử lý kết nối SignalR
        private async void ConnectSignalR()
        {
            // Lưu ý: Thay 7200 bằng đúng cái cổng WebAPI của bạn
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7200/notificationHub")
                .WithAutomaticReconnect()
                .Build();

            // Lắng nghe tín hiệu "ReceiveNotification" từ Controller bắn về
            connection.On<string>("ReceiveNotification", (message) =>
            {
                // Cập nhật giao diện an toàn thông qua Dispatcher
                Dispatcher.Invoke(() =>
                {
                    LoadData(); // Tải lại bảng để thấy đơn mới
                    MessageBox.Show(message, "🔔 ĐƠN HÀNG MỚI!", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                // Bỏ qua nếu chưa bật WebAPI
            }
        }
        // Sự kiện: Khi click vào 1 dòng trên bảng, tự động điền data sang Form bên trái
        private void dgvGoiCuoc_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgvGoiCuoc.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgvGoiCuoc.SelectedItem;
                txtIdGoi.Text = row["Id"].ToString();
                txtTenGoi.Text = row["TenGoi"].ToString();
                cbxLoaiDichVu.Text = row["LoaiDichVu"].ToString();
                txtGiaTien.Text = Convert.ToDecimal(row["GiaTien"]).ToString("0"); // Bỏ số thập phân
                txtMoTa.Text = row["MoTa"].ToString();
            }
        }

        // Nút: Làm mới Form (Xóa trắng textboxes)
        private void btnLamMoiForm_Click(object sender, RoutedEventArgs e)
        {
            txtIdGoi.Clear();
            txtTenGoi.Clear();
            cbxLoaiDichVu.SelectedIndex = 0;
            txtGiaTien.Clear();
            txtMoTa.Clear();
            dgvGoiCuoc.SelectedItem = null;
        }

        // Nút: Thêm mới
        private void btnThemGC_Click(object sender, RoutedEventArgs e)
        {
            decimal gia;
            if (!decimal.TryParse(txtGiaTien.Text, out gia))
            {
                MessageBox.Show("Giá tiền phải là con số hợp lệ!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string error;
            if (gcBLL.ThemGoiCuoc(txtTenGoi.Text, cbxLoaiDichVu.Text, gia, txtMoTa.Text, out error))
            {
                MessageBox.Show("Thêm gói cước thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataGoiCuoc();
                btnLamMoiForm_Click(null, null);
            }
            else
            {
                MessageBox.Show(error, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Nút: Sửa
        private void btnSuaGC_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdGoi.Text))
            {
                MessageBox.Show("Vui lòng chọn 1 gói cước trên bảng để sửa!", "Chú ý", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal gia;
            decimal.TryParse(txtGiaTien.Text, out gia);
            string error;

            if (gcBLL.SuaGoiCuoc(int.Parse(txtIdGoi.Text), txtTenGoi.Text, cbxLoaiDichVu.Text, gia, txtMoTa.Text, out error))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataGoiCuoc();
            }
            else
            {
                MessageBox.Show(error, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Nút: Xóa (Ẩn)
        private void btnXoaGC_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdGoi.Text)) return;

            if (MessageBox.Show("Bạn có chắc muốn ngừng bán gói cước này? Nó sẽ bị ẩn khỏi trang web khách hàng.", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (gcBLL.XoaGoiCuoc(int.Parse(txtIdGoi.Text)))
                {
                    MessageBox.Show("Đã ngừng bán gói cước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGoiCuoc();
                    btnLamMoiForm_Click(null, null);
                }
            }
        }
        private void LoadData()
        {
            // Lấy DataTable từ BLL và nhét vào DataGrid
            dgvKhachHang.ItemsSource = khBLL.LayDanhSachKhachHang().DefaultView;
            LoadThongKe();
        }
        private void LoadThongKe()
        {
            DataTable dt = khBLL.LayThongKeTrangThai();
            int choXuLy = 0, dangKhaoSat = 0, hoanThanh = 0, daHuy = 0;

            // Bóc tách dữ liệu từ CSDL
            foreach (DataRow row in dt.Rows)
            {
                int trangThai = Convert.ToInt32(row["TrangThai"]);
                int soLuong = Convert.ToInt32(row["SoLuong"]);

                if (trangThai == 0) choXuLy = soLuong;
                else if (trangThai == 1) dangKhaoSat = soLuong;
                else if (trangThai == 2) hoanThanh = soLuong;
                else if (trangThai == 3) daHuy = soLuong;
            }

            int tong = choXuLy + dangKhaoSat + hoanThanh + daHuy;

            // Gán số vào các thẻ Card
            txtTongKhach.Text = tong.ToString();
            txtDaHoanThanh.Text = hoanThanh.ToString();
            txtDangCho.Text = (choXuLy + dangKhaoSat).ToString();

            // Vẽ biểu đồ tròn
            SeriesCollection series = new SeriesCollection();

            if (choXuLy > 0)
                series.Add(new PieSeries { Title = "Chờ xử lý", Values = new ChartValues<int> { choXuLy }, Fill = System.Windows.Media.Brushes.Orange, DataLabels = true });
            if (dangKhaoSat > 0)
                series.Add(new PieSeries { Title = "Đang khảo sát", Values = new ChartValues<int> { dangKhaoSat }, Fill = System.Windows.Media.Brushes.DeepSkyBlue, DataLabels = true });
            if (hoanThanh > 0)
                series.Add(new PieSeries { Title = "Hoàn thành", Values = new ChartValues<int> { hoanThanh }, Fill = System.Windows.Media.Brushes.LimeGreen, DataLabels = true });
            if (daHuy > 0)
                series.Add(new PieSeries { Title = "Đã hủy", Values = new ChartValues<int> { daHuy }, Fill = System.Windows.Media.Brushes.Red, DataLabels = true });

            pieChartTrangThai.Series = series;
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void MenuTrangThai_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem người dùng có đang chọn dòng nào không
            if (dgvKhachHang.SelectedItem == null) return;

            // Lấy dữ liệu của dòng đang chọn
            DataRowView row = (DataRowView)dgvKhachHang.SelectedItem;
            int idKhachHang = Convert.ToInt32(row["Id"]);

            // Lấy giá trị Tag của nút menu vừa bấm (0, 1, 2, 3)
            System.Windows.Controls.MenuItem menu = sender as System.Windows.Controls.MenuItem;
            int trangThaiMoi = Convert.ToInt32(menu.Tag);

            // Gọi BLL để cập nhật
            if (khBLL.CapNhatTrangThaiKhach(idKhachHang, trangThaiMoi))
            {
                MessageBox.Show("Đã cập nhật trạng thái thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Tải lại bảng dữ liệu để thấy sự thay đổi
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                // 2. Mở hộp thoại để người dùng chọn nơi lưu file Excel
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Lưu báo cáo khách hàng";
                saveFileDialog.FileName = "BaoCao_KhachHang_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(saveFileDialog.FileName);

                    // 3. Lấy dữ liệu mới nhất từ CSDL qua tầng BLL
                    DataTable dt = khBLL.LayDanhSachKhachHang();

                    // 4. Khởi tạo và ghi file Excel
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        // Tạo một trang tính (Sheet) mới
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("DanhSachKhachHang");

                        // Đổ toàn bộ dữ liệu từ DataTable vào Excel (Load bắt đầu từ ô A1, in luôn dòng tiêu đề)
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                        // Trang trí một chút cho dòng Tiêu đề (Header)
                        using (ExcelRange headerRange = worksheet.Cells[1, 1, 1, dt.Columns.Count])
                        {
                            headerRange.Style.Font.Bold = true; // In đậm
                            headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Tô nền xám
                        }

                        // Tự động căn chỉnh độ rộng các cột cho vừa với chữ
                        worksheet.Cells.AutoFitColumns();

                        // Lưu file lại
                        package.Save();
                    }

                    // 5. Báo thành công
                    MessageBox.Show("Đã xuất báo cáo Excel thành công!", "Tuyệt vời", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất file Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}