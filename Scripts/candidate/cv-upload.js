// Kiểm tra file upload phía client
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var form = document.getElementById('cvUploadForm');
        var fileInput = document.getElementById('cvFileInput');
        var errorSpan = document.getElementById('cvFileError');

        if (!form || !fileInput || !errorSpan) return;

        fileInput.addEventListener('change', validateFile);

        form.addEventListener('submit', function (e) {
            if (!validateFile()) {
                e.preventDefault();
            }
        });

        function validateFile() {
            errorSpan.textContent = '';
            var file = fileInput.files[0];

            if (!file) {
                return true;
            }

            // 1. Kiểm tra định dạng đuôi file
            var allowedExtensions = /(\.pdf|\.doc|\.docx)$/i;
            if (!allowedExtensions.exec(file.name)) {
                errorSpan.textContent = 'Định dạng file không hợp lệ. Chỉ chấp nhận .pdf, .doc, .docx';
                return false;
            }

            // 2. Kiểm tra dung lượng (Tối đa 3 MB = 3 * 1024 * 1024 bytes)
            var maxSizeInBytes = 3 * 1024 * 1024;
            if (file.size > maxSizeInBytes) {
                errorSpan.textContent = 'Dung lượng file vượt quá giới hạn 3 MB.';
                return false;
            }

            return true;
        }

        // ================= Quản lý Preview & Chi tiết CV =================
        var modalEl = document.getElementById('cvDetailModal');
        if (modalEl && typeof bootstrap !== 'undefined') {
            var detailModal = new bootstrap.Modal(modalEl);

            // Bắt sự kiện nháy đúp (double-click) trên hàng CV
            var cvRows = document.querySelectorAll('.cv-table-row');
            cvRows.forEach(function (row) {
                row.addEventListener('dblclick', function (e) {
                    // Không kích hoạt nếu bấm trúng nút thao tác hoặc form
                    if (e.target.closest('button') || e.target.closest('a') || e.target.closest('form')) {
                        return;
                    }
                    openCvModal(row);
                });
            });

            // Bắt sự kiện click vào nút Xem chi tiết / Preview
            var previewButtons = document.querySelectorAll('.btn-preview-cv');
            previewButtons.forEach(function (btn) {
                btn.addEventListener('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    var row = btn.closest('.cv-table-row');
                    if (row) {
                        openCvModal(row);
                    }
                });
            });

            // Ngăn chặn double-click bị kích hoạt ngoài ý muốn khi tương tác nhóm nút thao tác
            var actionElements = document.querySelectorAll('.cv-action-group a, .cv-action-group button');
            actionElements.forEach(function (el) {
                el.addEventListener('dblclick', function (e) {
                    e.stopPropagation();
                });
            });

            // Hàm đổ dữ liệu và mở Modal
            function openCvModal(row) {
                var title = row.getAttribute('data-title') || 'CV';
                var fileType = (row.getAttribute('data-filetype') || 'PDF').toUpperCase();
                var fileSize = row.getAttribute('data-filesize') || 'N/A';
                var uploaded = row.getAttribute('data-uploaded') || '';
                var isDefault = row.getAttribute('data-isdefault') === 'true';
                var appCount = row.getAttribute('data-appcount') || '0';
                var previewUrl = row.getAttribute('data-preview-url') || '#';
                var downloadUrl = row.getAttribute('data-download-url') || '#';

                // Gán dữ liệu hiển thị
                var modalTitle = document.getElementById('cvDetailModalLabel');
                if (modalTitle) modalTitle.textContent = 'Chi tiết CV: ' + title;

                var fileTypeEl = document.getElementById('modalCvFileType');
                if (fileTypeEl) fileTypeEl.textContent = fileType;

                var fileSizeEl = document.getElementById('modalCvFileSize');
                if (fileSizeEl) fileSizeEl.textContent = fileSize;

                var uploadedEl = document.getElementById('modalCvUploadedAt');
                if (uploadedEl) uploadedEl.textContent = uploaded;

                var appCountEl = document.getElementById('modalCvAppCount');
                if (appCountEl) appCountEl.textContent = appCount + ' tin tuyển dụng';

                var badgeDefault = document.getElementById('modalCvDefaultBadge');
                if (badgeDefault) {
                    if (isDefault) {
                        badgeDefault.classList.remove('d-none');
                    } else {
                        badgeDefault.classList.add('d-none');
                    }
                }

                var iconContainer = document.getElementById('modalCvIcon');
                var iframe = document.getElementById('modalCvIframe');
                var nonPdfNotice = document.getElementById('modalCvNonPdfNotice');
                var loading = document.getElementById('modalCvLoading');
                var openTabBtn = document.getElementById('modalOpenTabBtn');
                var downloadBtn = document.getElementById('modalDownloadBtn');
                var wordDownloadBtn = document.getElementById('modalWordDownloadBtn');

                if (openTabBtn) openTabBtn.href = previewUrl;
                if (downloadBtn) downloadBtn.href = downloadUrl;
                if (wordDownloadBtn) wordDownloadBtn.href = downloadUrl;

                if (fileType === 'PDF') {
                    if (iconContainer) iconContainer.innerHTML = '<i class="bi bi-file-earmark-pdf-fill text-danger"></i>';
                    if (nonPdfNotice) nonPdfNotice.classList.add('d-none');
                    if (openTabBtn) openTabBtn.classList.remove('d-none');
                    if (iframe) {
                        iframe.classList.add('d-none');
                        if (loading) loading.classList.remove('d-none');

                        iframe.onload = function () {
                            if (loading) loading.classList.add('d-none');
                            iframe.classList.remove('d-none');
                        };
                        iframe.src = previewUrl;
                    }
                } else {
                    if (iconContainer) iconContainer.innerHTML = '<i class="bi bi-file-earmark-word-fill text-primary"></i>';
                    if (loading) loading.classList.add('d-none');
                    if (iframe) {
                        iframe.src = 'about:blank';
                        iframe.classList.add('d-none');
                    }
                    if (nonPdfNotice) nonPdfNotice.classList.remove('d-none');
                    if (openTabBtn) openTabBtn.classList.add('d-none');
                }

                detailModal.show();
            }

            // Dọn sạch iframe khi đóng modal để giải phóng bộ nhớ
            modalEl.addEventListener('hidden.bs.modal', function () {
                var iframe = document.getElementById('modalCvIframe');
                if (iframe) {
                    iframe.src = 'about:blank';
                    iframe.classList.add('d-none');
                }
            });
        }

        // Khởi tạo tooltips
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.forEach(function (tooltipTriggerEl) {
                new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    });
})();