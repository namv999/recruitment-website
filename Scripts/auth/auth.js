/**
 * Content/js/auth.js
 * JavaScript logic for Login and Register pages
 */

$(function () {
    // --- 1. Dynamic Employer Fields Toggle on Register Page ---
    function updateRoleVisibility(animate) {
        var selectedRole = $('input[name="Role"]:checked').val();
        var $employerFields = $('#employerFields');

        if ($employerFields.length === 0) return;

        if (selectedRole === 'employer') {
            if (animate) {
                $employerFields.stop(true, true).slideDown(250);
            } else {
                $employerFields.show();
            }
            $('#companyNameInput').prop('disabled', false);
            $('#taxCodeInput').prop('disabled', false);
        } else {
            if (animate) {
                $employerFields.stop(true, true).slideUp(250);
            } else {
                $employerFields.hide();
            }
            // Candidate role selected
            $('#companyNameInput').prop('disabled', false);
            $('#taxCodeInput').prop('disabled', false);
        }
    }

    // Initial check on page load (e.g. after POST validation error return or fresh load)
    if ($('input[name="Role"]').length > 0) {
        // If none is checked, default to candidate
        if ($('input[name="Role"]:checked').length === 0) {
            $('#roleCandidate').prop('checked', true);
        }
        updateRoleVisibility(false);

        // Listen for changes on Role radio buttons
        $('input[name="Role"]').on('change', function () {
            updateRoleVisibility(true);
        });
    }

    // --- 2. Password Toggle Visibility Feature ---
    $('.btn-password-toggle').on('click', function () {
        var $btn = $(this);
        var $input = $btn.closest('.input-group').find('input');
        var isPassword = $input.attr('type') === 'password';

        $input.attr('type', isPassword ? 'text' : 'password');

        // Toggle icon visually
        var $iconShow = $btn.find('.icon-eye-show');
        var $iconHide = $btn.find('.icon-eye-hide');

        if (isPassword) {
            $iconShow.addClass('d-none');
            $iconHide.removeClass('d-none');
            $btn.attr('title', 'Ẩn mật khẩu');
        } else {
            $iconShow.removeClass('d-none');
            $iconHide.addClass('d-none');
            $btn.attr('title', 'Hiện mật khẩu');
        }
    });

    // --- 3. Clean validation errors on input focus/typing ---
    $('.auth-form input').on('input', function () {
        var $this = $(this);
        $this.removeClass('input-validation-error');
        $this.siblings('.field-validation-error').fadeOut(200);
    });
});
