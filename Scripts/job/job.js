/**
 * Scripts/job/job.js
 * Module: Job Listings & Search
 */
(function () {
    'use strict';

    // Form có thuộc tính data-confirm="..." sẽ hỏi xác nhận trước khi gửi (Tạm dừng / Đóng / Mở lại tin)
    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form || typeof form.getAttribute !== 'function') return;

        var message = form.getAttribute('data-confirm');
        if (message && !window.confirm(message)) {
            e.preventDefault();
        }
    });

    // ---- Ô nhập ngày dd/mm/yyyy (form đăng / sửa tin) ----
    // Ô hiển thị: [data-date-input]. Ô ẩn (id = data-date-target) giữ giá trị yyyy-MM-dd để gửi lên server,
    // nên không phụ thuộc ngôn ngữ trình duyệt hay culture của server.
    function pad(n) { return (n < 10 ? '0' : '') + n; }

    function toIso(text) {
        var m = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(text);
        if (!m) return null;

        var day = parseInt(m[1], 10), month = parseInt(m[2], 10), year = parseInt(m[3], 10);
        var date = new Date(year, month - 1, day);
        if (date.getFullYear() !== year || date.getMonth() !== month - 1 || date.getDate() !== day) return null;

        return year + '-' + pad(month) + '-' + pad(day);
    }

    function autoSlash(value) {
        var digits = value.replace(/\D/g, '').slice(0, 8);
        var out = digits.slice(0, 2);
        if (digits.length > 2) out += '/' + digits.slice(2, 4);
        if (digits.length > 4) out += '/' + digits.slice(4);
        return out;
    }

    function setupDateInput(input) {
        var target = document.getElementById(input.getAttribute('data-date-target'));
        var error = input.parentNode.querySelector('[data-date-error]');
        if (!target) return;

        function showError(message) {
            if (!error) return;
            error.textContent = message;
            error.hidden = false;
        }

        function clearError() {
            if (!error) return;
            error.textContent = '';
            error.hidden = true;
        }

        // Đồng bộ ô ẩn; trả về false nếu người dùng đã nhập nhưng không phải ngày hợp lệ
        function sync() {
            var text = input.value.trim();
            if (text === '') {
                target.value = '';
                clearError();
                return true;
            }

            var iso = toIso(text);
            if (iso) {
                target.value = iso;
                clearError();
                return true;
            }

            target.value = '';
            return false;
        }

        input.addEventListener('input', function () {
            input.value = autoSlash(input.value);
            if (input.value.length === 10) {
                sync();
            } else {
                target.value = '';
            }
        });

        input.addEventListener('blur', function () {
            if (!sync()) showError('Ngày không hợp lệ. Nhập theo dạng dd/mm/yyyy, ví dụ 31/12/2026.');
        });

        if (input.form) {
            input.form.addEventListener('submit', function (e) {
                if (!sync()) {
                    e.preventDefault();
                    showError('Ngày không hợp lệ. Nhập theo dạng dd/mm/yyyy, ví dụ 31/12/2026.');
                    input.focus();
                }
            });
        }
    }

    Array.prototype.forEach.call(document.querySelectorAll('[data-date-input]'), setupDateInput);
})();
