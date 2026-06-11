using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Collections.Generic;
using BLL;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoiCuocController : ControllerBase
    {
        private GoiCuocBLL gcBLL = new GoiCuocBLL();

        // Đường dẫn API sẽ là: GET /api/goicuoc/danh-sach
        [HttpGet("danh-sach")]
        public IActionResult GetDanhSach()
        {
            try
            {
                DataTable dt = gcBLL.LayDanhSachGoiCuoc();
                var listGoiCuoc = new List<object>();

                // Convert DataTable thành List Object để xuất JSON cho đẹp
                foreach (DataRow row in dt.Rows)
                {
                    listGoiCuoc.Add(new
                    {
                        Id = row["Id"],
                        TenGoi = row["TenGoi"].ToString(),
                        LoaiDichVu = row["LoaiDichVu"].ToString(),
                        GiaTien = Convert.ToDecimal(row["GiaTien"]),
                        MoTa = row["MoTa"].ToString()
                    });
                }

                return Ok(new { success = true, data = listGoiCuoc });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy dữ liệu: " + ex.Message });
            }
        }
    }
}