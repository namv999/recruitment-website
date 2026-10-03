// Xử lý AJAX Toggle Lưu tin tuyển dụng (Dành cho cả trang danh sách & chi tiết)
(function ($) {
    'use strict';

    $(document).ready(function () {
        $(document).on('click', '.saved-toggle-btn', function (e) {
            e.preventDefault();

            var $btn = $(this);
            var jobId = $btn.data('job-id');
            var token = $('input[name="__RequestVerificationToken"]').val();

            if (!jobId) return;

            $.ajax({
                url: '/SavedJob/Toggle',
                type: 'POST',
                data: {
                    jobId: jobId,
                    __RequestVerificationToken: token
                },
                success: function (res) {
                    if (res && res.success) {
                        if (res.saved) {
                            $btn.addClass('active').attr('aria-pressed', 'true').attr('aria-label', 'Bỏ lưu tin');
                            $btn.find('.saved-icon').removeClass('bi-heart').addClass('bi-heart-fill text-danger');
                        } else {
                            $btn.removeClass('active').attr('aria-pressed', 'false').attr('aria-label', 'Lưu tin');
                            $btn.find('.saved-icon').removeClass('bi-heart-fill text-danger').addClass('bi-heart');
                        }
                    } else if (res && res.message) {
                        alert(res.message);
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra, vui lòng thử lại sau.');
                }
            });
        });
    });
})(jQuery);