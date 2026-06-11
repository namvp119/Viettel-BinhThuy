// main.js - DARK RED GLOW VERSION

document.addEventListener("DOMContentLoaded", function() {
    // Khởi tạo AOS
    AOS.init({ duration: 800, once: true, offset: 50 });

    // Khởi tạo Tilt
    if (typeof VanillaTilt !== 'undefined') {
        VanillaTilt.init(document.querySelectorAll("[data-tilt]"), {
            max: 15, speed: 400, glare: true, "max-glare": 0.2,
        });
    }

    // Custom Cursor Logic
    const cursorDot = document.querySelector('.cursor-dot');
    const cursorOutline = document.querySelector('.cursor-outline');
    if (cursorDot && cursorOutline) {
        window.addEventListener('mousemove', function(e) {
            cursorDot.style.left = `${e.clientX}px`; 
            cursorDot.style.top = `${e.clientY}px`;
            cursorOutline.animate({ left: `${e.clientX}px`, top: `${e.clientY}px` }, { duration: 200, fill: "forwards" });
        });
        document.querySelectorAll('.hover-target, a, button, input, select, .tab-btn').forEach(target => {
            target.addEventListener('mouseenter', () => { cursorDot.classList.add('hover'); cursorOutline.classList.add('hover'); });
            target.addEventListener('mouseleave', () => { cursorDot.classList.remove('hover'); cursorOutline.classList.remove('hover'); });
        });
    }

    // Number Counters
    const counters = document.querySelectorAll('.counter');
    const animateCounters = () => {
        counters.forEach(counter => {
            const updateCount = () => {
                const target = +counter.getAttribute('data-target');
                const count = +counter.innerText;
                const inc = target / 150;
                if (count < target) { counter.innerText = Math.ceil(count + inc); setTimeout(updateCount, 15); } 
                else { counter.innerText = target; }
            };
            updateCount();
        });
    };
    const statsSection = document.getElementById('stats-section');
    if (statsSection) {
        new IntersectionObserver((entries, obs) => {
            if(entries[0].isIntersecting) { animateCounters(); obs.disconnect(); }
        }, { threshold: 0.5 }).observe(statsSection);
    }

    // FAQ Accordion Logic
    const faqBtns = document.querySelectorAll('.faq-btn');
    faqBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const content = this.nextElementSibling;
            const icon = this.querySelector('.faq-icon');
            
            // Đóng tất cả tab khác
            document.querySelectorAll('.faq-content').forEach(item => {
                if (item !== content) {
                    item.style.maxHeight = null;
                    const prevIcon = item.previousElementSibling.querySelector('.faq-icon');
                    if(prevIcon) {
                        prevIcon.textContent = '+';
                        prevIcon.style.transform = 'rotate(0deg)';
                    }
                }
            });

            // Mở tab hiện tại
            if (content.style.maxHeight) {
                content.style.maxHeight = null;
                icon.textContent = '+';
                icon.style.transform = 'rotate(0deg)';
            } else {
                content.style.maxHeight = content.scrollHeight + "px";
                icon.textContent = '−'; // Chuyển thành dấu trừ
                icon.style.transform = 'rotate(90deg)';
            }
        });
    });

    // Xử lý Form
    // Xử lý Form với WebAPI thực tế
    const form = document.getElementById('contactForm');
    const submitBtn = document.getElementById('submitBtn');
    const btnText = document.getElementById('btnText');
    const spinner = document.getElementById('loadingSpinner');
    const toast = document.getElementById('toast');

    if(form) {
        // Đổi thành async function để gọi API
        form.addEventListener('submit', async function(e) {
            e.preventDefault();
            if (submitBtn && btnText && spinner) {
                const originalText = btnText.textContent;
                submitBtn.disabled = true;
                btnText.textContent = "Đang kết nối...";
                spinner.classList.remove('hidden');
                
                // 1. Lấy dữ liệu người dùng nhập
                const hoTen = document.getElementById('floating_name').value;
                const soDienThoai = document.getElementById('floating_phone').value;
                // Thêm check an toàn nếu bạn không dùng 2 ô mới
                const nhuCau = document.getElementById('floating_nhucau') ? document.getElementById('floating_nhucau').value : "";
                const diaChi = document.getElementById('floating_diachi') ? document.getElementById('floating_diachi').value : "";

                // Gói thành cục Object JSON để gửi
                const payload = {
                    HoTen: hoTen,
                    SoDienThoai: soDienThoai,
                    NhuCau: nhuCau,
                    DiaChi: diaChi
                };

                try {
                    // 2. GỌI API (Lưu ý: Bạn phải chạy project WebAPI bên Visual Tím, xem port nó cấp là bao nhiêu để thay thế vào chữ cổng_của_bạn bên dưới)
                    const response = await fetch('https://localhost:7200/api/khachhang/dang-ky', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(payload)
                    });

                    const result = await response.json();

                    // 3. Xử lý kết quả trả về từ C#
                    if (response.ok && result.success) {
                        form.reset();
                        if(toast) {
                            // Hiện popup xanh thành công
                            toast.querySelector('h4').textContent = "Đăng ký thành công!";
                            toast.querySelector('p').textContent = result.message;
                            toast.classList.add('show');
                            setTimeout(() => { toast.classList.remove('show'); }, 4000);
                        }
                    } else {
                        // Báo lỗi bằng alert nếu nhập thiếu tên/SĐT
                        alert("Thất bại: " + result.message);
                    }
                } catch (error) {
                    alert("Lỗi kết nối máy chủ! Hãy chắc chắn project WebAPI đang được chạy.");
                    console.error("API Fetch Error:", error);
                } finally {
                    // Trả lại trạng thái ban đầu cho nút bấm
                    submitBtn.disabled = false;
                    btnText.textContent = originalText;
                    spinner.classList.add('hidden');
                }
            }
        });
    }
});
// HÀM TỰ ĐỘNG ĐỒNG BỘ GIÁ & VẼ THÊM GÓI CƯỚC MỚI TỪ SQL SERVER
    async function dongBoGiaGoiCuoc() {
        try {
            // Thay 7200 bằng đúng cổng WebAPI của bạn
            const response = await fetch('https://localhost:7200/api/goicuoc/danh-sach');
            const result = await response.json();

            if (response.ok && result.success) {
                result.data.forEach(goi => {
                    // 1. Định dạng giá tiền
                    let giaHienThi = "";
                    if (goi.loaiDichVu === "Wifi" || goi.loaiDichVu === "DoanhNghiep") {
                        if (goi.giaTien >= 1000000) {
                            giaHienThi = (goi.giaTien / 1000).toLocaleString('vi-VN') + 'K';
                        } else {
                            giaHienThi = (goi.giaTien / 1000) + 'K';
                        }
                    } else {
                        giaHienThi = goi.giaTien.toLocaleString('vi-VN') + 'đ';
                    }

                    // 2. Tìm xem gói cước này đã có thẻ HTML trên web chưa
                    const elements = document.querySelectorAll(`[data-pkg-price="${goi.tenGoi}"]`);
                    
                    if (elements.length > 0) {
                        // NẾU LÀ GÓI CŨ: Chỉ cập nhật đè giá tiền mới vào
                        elements.forEach(el => el.textContent = giaHienThi);
                    } else {
                        // NẾU LÀ GÓI MỚI HOÀN TOÀN: Tự động vẽ thêm Card HTML dựa trên Loại Dịch Vụ
                        let containerId = "";
                        let htmlTemplate = "";

                        if (goi.loaiDichVu === "Wifi") {
                            containerId = "tab-wifi";
                            htmlTemplate = `
                            <div class="neon-card bg-white p-10 rounded-[2.5rem] shadow-xl border-2 border-gray-100 hover-target cursor-none group mt-6" data-aos="fade-up">
                                <p class="font-black text-vtdark text-3xl group-hover:text-[#FF0033] group-hover:text-glow-red-small transition-all">${goi.tenGoi} <span class="text-base font-bold text-gray-400 block mt-2 drop-shadow-none">(${goi.moTa || 'Gói cước mới'})</span></p>
                                <p class="text-base font-bold text-gray-600 mt-6 mb-8 border-b-2 border-gray-100 pb-6 flex items-center gap-3"><svg class="w-6 h-6 text-[#00CC44]" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="4" d="M5 13l4 4L19 7"></path></svg> Dịch vụ Internet Cáp Quang</p>
                                <p class="text-5xl text-vtdark font-black group-hover:text-glow-white-small transition-all"><span data-pkg-price="${goi.tenGoi}">${giaHienThi}</span><span class="text-lg font-bold text-gray-400 drop-shadow-none">/tháng</span></p>
                            </div>`;
                        } else if (goi.loaiDichVu === "TV") {
                            containerId = "tab-tv";
                            htmlTemplate = `
                            <div class="neon-card bg-white p-8 rounded-[2rem] shadow-lg border border-gray-100 hover-target group mt-6" data-aos="fade-up">
                                <p class="font-black text-xl group-hover:text-[#FF0033] group-hover:text-glow-red-small transition-all">${goi.tenGoi}</p>
                                <p class="text-sm font-bold text-gray-500 mt-2">${goi.moTa || 'Trang bị đầu thu VIP'}</p>
                                <p class="text-3xl text-vtdark font-black mt-4 group-hover:text-glow-white-small transition-all" data-pkg-price="${goi.tenGoi}">${giaHienThi}</p>
                            </div>`;
                        } else if (goi.loaiDichVu === "Camera") {
                            containerId = "tab-camera";
                            htmlTemplate = `
                            <div class="neon-card bg-white p-8 rounded-[2rem] shadow-lg border border-gray-100 hover-target group mt-6" data-aos="fade-up">
                                <p class="font-black text-xl text-vtdark group-hover:text-[#FF0033] group-hover:text-glow-red-small transition-all">${goi.tenGoi}</p>
                                <p class="text-sm font-bold text-gray-500 mt-2">${goi.moTa || 'Camera AI Thông Minh'}</p>
                                <p class="text-3xl text-vtdark font-black mt-4 group-hover:text-glow-white-small transition-all" data-pkg-price="${goi.tenGoi}">${giaHienThi}</p>
                            </div>`;
                        } else if (goi.loaiDichVu === "DoanhNghiep") {
                            containerId = "tab-doanhnghiep";
                            htmlTemplate = `
                            <div class="bg-white/5 p-8 rounded-3xl backdrop-blur-2xl border-t border-l border-white/20 hover-target shadow-xl mt-6" data-aos="fade-up">
                                <p class="text-[#FF3366] font-black uppercase text-xs mb-6 tracking-widest border-b border-white/10 pb-4 text-glow-red-small">🔥 GÓI CƯỚC MỚI</p>
                                <div class="space-y-5 text-white">
                                    <div class="flex justify-between items-end"><span class="font-bold text-base">${goi.tenGoi} <span class="block text-xs text-gray-400 font-semibold mt-1">${goi.moTa || ''}</span></span> <span class="font-black text-[#FF3366] text-2xl text-glow-red-small" data-pkg-price="${goi.tenGoi}">${giaHienThi}</span></div>
                                </div>
                            </div>`;
                        }

                        // 3. Chèn khối HTML vừa tạo vào đúng Tab trên màn hình
                        if (containerId) {
                            const tabElement = document.getElementById(containerId);
                            if (tabElement) {
                                // Tìm thẻ grid gần nhất bên trong tab để ném card vào đó
                                const gridContainer = tabElement.querySelector('.grid');
                                if (gridContainer) {
                                    gridContainer.insertAdjacentHTML('beforeend', htmlTemplate);
                                }
                            }
                        }
                    }
                });
                console.log("⚡ Đã đồng bộ và Tự động vẽ các gói cước MỚI từ database thành công!");
            }
        } catch (error) {
            console.error("❌ Lỗi load dynamic pricing:", error);
        }
    }

    // Vẫn giữ nguyên lệnh gọi hàm này ở cuối nhé
    dongBoGiaGoiCuoc();
function openTab(evt, tabName) {
    Array.from(document.getElementsByClassName("tab-content")).forEach(t => t.classList.remove("active"));
    Array.from(document.getElementsByClassName("tab-btn")).forEach(t => t.classList.remove("active"));
    document.getElementById(tabName).classList.add("active");
    evt.currentTarget.classList.add("active");
}
