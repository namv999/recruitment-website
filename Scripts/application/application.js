// Vanilla JS/jQuery hỗ trợ tương tác Module Application
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        // 1. Đếm ký tự Thư giới thiệu
        var coverLetterInput = document.getElementById('coverLetterInput');
        var coverLetterCounter = document.getElementById('coverLetterCounter');

        if (coverLetterInput && coverLetterCounter) {
            var updateCounter = function () {
                var len = coverLetterInput.value ? coverLetterInput.value.length : 0;
                coverLetterCounter.textContent = len + ' / 2000 ký tự';
            };
            coverLetterInput.addEventListener('input', updateCounter);
            updateCounter();
        }

        // 2. Chặn bấm đúp form Apply
        var applyForm = document.getElementById('applyForm');
        var applySubmitBtn = document.getElementById('applySubmitBtn');

        if (applyForm && applySubmitBtn) {
            applyForm.addEventListener('submit', function () {
                applySubmitBtn.disabled = true;
                applySubmitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang gửi...';
            });
        }

        // 3. Confirm Rút đơn
        var withdrawForms = document.querySelectorAll('.app-withdraw-form');
        withdrawForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!confirm('Bạn chắc chắn muốn rút đơn ứng tuyển này? Hành động này không thể hoàn tác.')) {
                    e.preventDefault();
                }
            });
        });
    });
})();