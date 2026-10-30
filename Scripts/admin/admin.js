/**
 * Scripts/admin/admin.js
 * JavaScript cho module Quản trị viên (Admin)
 * Tuân thủ CONVENTIONS.md, ADMIN-FE-BE-CONTRACT.md và SKILL.md
 */

document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    // 1. Xử lý xác nhận cho các form thao tác (Toggle status, Xác minh, Xóa, Duyệt tin)
    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form) return;

        // Kiểm tra thuộc tính data-confirm trên form
        var message = form.getAttribute('data-confirm');
        if (message && !window.confirm(message)) {
            e.preventDefault();
            return false;
        }
    });

    // Cũng hỗ trợ click trực tiếp vào nút có data-confirm (phòng trường hợp nút ghi đè thông báo)
    document.addEventListener('click', function (e) {
        var btn = e.target.closest('button[data-confirm], input[type="submit"][data-confirm]');
        if (btn) {
            var msg = btn.getAttribute('data-confirm');
            if (msg && !window.confirm(msg)) {
                e.preventDefault();
                return false;
            }
        }
    });

    // 2. Modal Quản lý Kỹ năng (Thêm / Chỉnh sửa)
    var modalSkillEl = document.getElementById('modalSkillForm');
    if (modalSkillEl) {
        var modalSkillTitle = document.getElementById('modalSkillTitle');
        var inputSkillId = document.getElementById('skillId');
        var inputSkillName = document.getElementById('skillName');
        var selectSkillCategory = document.getElementById('skillCategoryId');

        // Nút mở modal "Thêm mới"
        var addSkillBtns = document.querySelectorAll('[data-action="add-skill"]');
        addSkillBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                if (modalSkillTitle) modalSkillTitle.textContent = 'Thêm kỹ năng mới';
                if (inputSkillId) inputSkillId.value = '';
                if (inputSkillName) inputSkillName.value = '';
                if (selectSkillCategory) selectSkillCategory.value = '';
            });
        });

        // Nút mở modal "Sửa"
        var editSkillBtns = document.querySelectorAll('[data-action="edit-skill"]');
        editSkillBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var id = btn.getAttribute('data-id') || '';
                var name = btn.getAttribute('data-name') || '';
                var categoryId = btn.getAttribute('data-category-id') || '';

                if (modalSkillTitle) modalSkillTitle.textContent = 'Chỉnh sửa kỹ năng';
                if (inputSkillId) inputSkillId.value = id;
                if (inputSkillName) inputSkillName.value = name;
                if (selectSkillCategory) selectSkillCategory.value = categoryId;
            });
        });
    }

    // 3. Modal Quản lý Nhóm Kỹ năng (Thêm / Chỉnh sửa)
    var modalSkillCatEl = document.getElementById('modalSkillCategory');
    if (modalSkillCatEl) {
        var modalSkillCatTitle = document.getElementById('modalSkillCatTitle');
        var inputSkillCatId = document.getElementById('skillCatId');
        var inputSkillCatName = document.getElementById('skillCatName');

        var addSkillCatBtns = document.querySelectorAll('[data-action="add-skill-cat"]');
        addSkillCatBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                if (modalSkillCatTitle) modalSkillCatTitle.textContent = 'Thêm nhóm kỹ năng';
                if (inputSkillCatId) inputSkillCatId.value = '';
                if (inputSkillCatName) inputSkillCatName.value = '';
            });
        });

        var editSkillCatBtns = document.querySelectorAll('[data-action="edit-skill-cat"]');
        editSkillCatBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var id = btn.getAttribute('data-id') || '';
                var name = btn.getAttribute('data-name') || '';

                if (modalSkillCatTitle) modalSkillCatTitle.textContent = 'Chỉnh sửa nhóm kỹ năng';
                if (inputSkillCatId) inputSkillCatId.value = id;
                if (inputSkillCatName) inputSkillCatName.value = name;
            });
        });
    }

    // 4. Modal Quản lý Ngành nghề (Job Categories) (Thêm / Chỉnh sửa)
    var modalJobCatEl = document.getElementById('modalJobCategory');
    if (modalJobCatEl) {
        var modalJobCatTitle = document.getElementById('modalJobCatTitle');
        var inputJobCatId = document.getElementById('jobCatId');
        var inputJobCatName = document.getElementById('jobCatName');

        var addJobCatBtns = document.querySelectorAll('[data-action="add-job-cat"]');
        addJobCatBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                if (modalJobCatTitle) modalJobCatTitle.textContent = 'Thêm ngành nghề tuyển dụng';
                if (inputJobCatId) inputJobCatId.value = '';
                if (inputJobCatName) inputJobCatName.value = '';
            });
        });

        var editJobCatBtns = document.querySelectorAll('[data-action="edit-job-cat"]');
        editJobCatBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var id = btn.getAttribute('data-id') || '';
                var name = btn.getAttribute('data-name') || '';

                if (modalJobCatTitle) modalJobCatTitle.textContent = 'Chỉnh sửa ngành nghề tuyển dụng';
                if (inputJobCatId) inputJobCatId.value = id;
                if (inputJobCatName) inputJobCatName.value = name;
            });
        });
    }
});
