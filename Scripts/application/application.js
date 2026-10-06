/* ==========================================================================
   Scripts/application/application.js
   Module: Applications & Status Tracking (Employer & Candidate)
   ========================================================================== */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {

        // ------------------------------------------------------------------
        // 1. Phía Ứng viên: Đếm ký tự Thư giới thiệu trong Form Apply
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        // 2. Phía Ứng viên: Chặn bấm đúp (Double Submit) Form Apply
        // ------------------------------------------------------------------
        var applyForm = document.getElementById('applyForm');
        var applySubmitBtn = document.getElementById('applySubmitBtn');

        if (applyForm && applySubmitBtn) {
            applyForm.addEventListener('submit', function () {
                applySubmitBtn.disabled = true;
                applySubmitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang gửi...';
            });
        }

        // ------------------------------------------------------------------
        // 3. Phía Ứng viên: Confirm Rút đơn ứng tuyển
        // ------------------------------------------------------------------
        var withdrawForms = document.querySelectorAll('.app-withdraw-form');
        withdrawForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!confirm('Bạn chắc chắn muốn rút đơn ứng tuyển này? Hành động này không thể hoàn tác.')) {
                    e.preventDefault();
                }
            });
        });

        // ------------------------------------------------------------------
        // 4. Phía Nhà tuyển dụng: Confirm các hành động cảnh báo (vd: Từ chối hồ sơ)
        // ------------------------------------------------------------------
        var confirmButtons = document.querySelectorAll('button[data-confirm]');
        confirmButtons.forEach(function (button) {
            button.addEventListener('click', function (e) {
                var message = this.getAttribute('data-confirm');
                if (message && !confirm(message)) {
                    e.preventDefault();
                }
            });
        });

    });
})();