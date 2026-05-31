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
    const form = document.getElementById('contactForm');
    const submitBtn = document.getElementById('submitBtn');
    const btnText = document.getElementById('btnText');
    const spinner = document.getElementById('loadingSpinner');
    const toast = document.getElementById('toast');

    if(form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            if (submitBtn && btnText && spinner) {
                const originalText = btnText.textContent;
                submitBtn.disabled = true;
                btnText.textContent = "Đang kết nối...";
                spinner.classList.remove('hidden');
                
                setTimeout(() => {
                    form.reset();
                    submitBtn.disabled = false;
                    btnText.textContent = originalText;
                    spinner.classList.add('hidden');
                    if(toast) {
                        toast.classList.add('show');
                        setTimeout(() => { toast.classList.remove('show'); }, 3000);
                    }
                }, 1200);
            }
        });
    }
});

function openTab(evt, tabName) {
    Array.from(document.getElementsByClassName("tab-content")).forEach(t => t.classList.remove("active"));
    Array.from(document.getElementsByClassName("tab-btn")).forEach(t => t.classList.remove("active"));
    document.getElementById(tabName).classList.add("active");
    evt.currentTarget.classList.add("active");
}
