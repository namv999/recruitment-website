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
    });
})();