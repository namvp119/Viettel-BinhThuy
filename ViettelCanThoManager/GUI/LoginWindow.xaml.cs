using System.Windows;
using BLL; // Gọi BLL

namespace GUI
{
    public partial class LoginWindow : Window
    {
        private TaiKhoanBLL tkBLL = new TaiKhoanBLL();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Password; // Thuộc tính Password của PasswordBox
            string error;

            if (tkBLL.KiemTraDangNhap(user, pass, out error))
            {
                // Mở cửa sổ chính
                MainWindow main = new MainWindow();
                main.Show();

                // Đóng cửa sổ đăng nhập hiện tại
                this.Close();
            }
            else
            {
                MessageBox.Show(error, "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}