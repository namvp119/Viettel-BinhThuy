using Microsoft.AspNetCore.Mvc;
using BLL; // Gọi tầng BLL lên để dùng
using Microsoft.AspNetCore.SignalR; // [THÊM MỚI]
using WebAPI.Hubs; // [THÊM MỚI] Gọi trạm phát sóng NotificationHub
using System.Threading.Tasks; // [THÊM MỚI] Để dùng async/await

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        // Khởi tạo đối tượng BLL
        private KhachHangBLL khBLL = new KhachHangBLL();

        // [THÊM MỚI] Khai báo Trạm phát sóng SignalR
        private readonly IHubContext<NotificationHub> _hubContext;

        // [THÊM MỚI] Constructor để Inject cái Hub vào Controller
        public KhachHangController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // Khai báo đường dẫn API là: POST /api/khachhang/dang-ky
        [HttpPost("dang-ky")]
        public async Task<IActionResult> DangKyTuVan([FromBody] KhachHangRequest request) // [SỬA] Đổi thành async Task
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            string errorMsg;
            // Đẩy dữ liệu xuống tầng BLL xử lý
            bool isSuccess = khBLL.DangKyTuVan(request.HoTen, request.SoDienThoai, request.NhuCau, request.DiaChi, out errorMsg);

            if (isSuccess)
            {
                // [THÊM MỚI] Bắn tín hiệu về phần mềm WPF ngay lập tức
                string thongBao = $"Khách hàng mới: {request.HoTen} - {request.SoDienThoai}";
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", thongBao);

                // Trả về JSON cho Frontend (main.js) báo thành công
                return Ok(new { success = true, message = "Đăng ký thành công! Kỹ thuật viên sẽ gọi lại ngay." });
            }
            else
            {
                // Trả về JSON báo lỗi (VD: sai SĐT, trống tên)
                return BadRequest(new { success = false, message = errorMsg });
            }
        }
    }

    // Class DTO (Data Transfer Object) dùng để hứng cục JSON từ file JS gửi lên
    public class KhachHangRequest
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string NhuCau { get; set; }
        public string DiaChi { get; set; }
    }
}