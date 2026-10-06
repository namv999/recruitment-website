/**
 * Scripts/shared/skill-selector.js
 * Component: _SkillSelector (dùng chung Job + Candidate) — Vanilla JS, không phụ thuộc thư viện.
 *
 * Hành vi:
 *  - Nút "Chọn kỹ năng" mở/đóng panel; đóng khi bấm ra ngoài hoặc nhấn Esc.
 *  - Ô tìm kiếm lọc theo tên (không phân biệt hoa thường và dấu).
 *  - Kỹ năng đã chọn hiện thành chip phía trên, bấm × để bỏ chọn.
 *  - Phát sự kiện "skillselector:change" (detail.ids = mảng id đã chọn) trên phần tử gốc.
 */
(function () {
    'use strict';

    function normalize(text) {
        var s = (text || '').toString().toLowerCase().replace(/đ/g, 'd');
        if (typeof s.normalize === 'function') {
            s = s.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
        }
        return s.trim();
    }

    function init(root) {
        if (root.getAttribute('data-initialized') === 'true') return;
        root.setAttribute('data-initialized', 'true');

        var toggle = root.querySelector('[data-skill-toggle]');
        var toggleLabel = root.querySelector('[data-skill-toggle-label]');
        var chips = root.querySelector('[data-skill-chips]');
        var search = root.querySelector('[data-skill-search]');
        var empty = root.querySelector('[data-skill-empty]');
        var options = Array.prototype.slice.call(root.querySelectorAll('[data-skill-option]'));
        var groups = Array.prototype.slice.call(root.querySelectorAll('[data-skill-group]'));

        function inputOf(option) { return option.querySelector('input[type="checkbox"]'); }
        function nameOf(option) { return option.querySelector('.skill-selector-option-name').textContent.trim(); }
        function selectedOptions() { return options.filter(function (o) { return inputOf(o).checked; }); }

        function renderChips() {
            var selected = selectedOptions();
            chips.textContent = '';

            if (selected.length === 0) {
                var placeholder = document.createElement('span');
                placeholder.className = 'skill-selector-placeholder';
                placeholder.textContent = 'Chưa chọn kỹ năng nào';
                chips.appendChild(placeholder);
            }

            selected.forEach(function (option) {
                var name = nameOf(option);

                var chip = document.createElement('span');
                chip.className = 'skill-selector-chip';
                chip.appendChild(document.createTextNode(name));

                var remove = document.createElement('button');
                remove.type = 'button';
                remove.className = 'skill-selector-chip-remove';
                remove.setAttribute('aria-label', 'Bỏ chọn ' + name);
                remove.textContent = '\u00d7';
                remove.addEventListener('click', function () {
                    var input = inputOf(option);
                    input.checked = false;
                    input.dispatchEvent(new Event('change', { bubbles: true }));
                });

                chip.appendChild(remove);
                chips.appendChild(chip);
            });

            toggleLabel.textContent = selected.length > 0 ? 'Thêm kỹ năng' : 'Chọn kỹ năng';
        }

        function isOpen() { return root.classList.contains('is-open'); }

        function open() {
            root.classList.add('is-open');
            toggle.setAttribute('aria-expanded', 'true');
            search.focus();
        }

        function close(returnFocus) {
            root.classList.remove('is-open');
            toggle.setAttribute('aria-expanded', 'false');
            if (returnFocus) toggle.focus();
        }

        function filter() {
            var keyword = normalize(search.value);
            var visibleCount = 0;

            options.forEach(function (option) {
                var match = keyword === '' || normalize(nameOf(option)).indexOf(keyword) !== -1;
                option.hidden = !match;
                if (match) visibleCount++;
            });

            groups.forEach(function (group) {
                var hasVisible = group.querySelector('[data-skill-option]:not([hidden])') !== null;
                group.hidden = !hasVisible;
            });

            empty.hidden = visibleCount > 0;
        }

        toggle.addEventListener('click', function () {
            if (isOpen()) { close(false); } else { open(); }
        });

        search.addEventListener('input', filter);

        // Nhấn Enter trong ô tìm kiếm không được submit form
        search.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') e.preventDefault();
        });

        root.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && isOpen()) {
                e.stopPropagation();
                close(true);
            }
        });

        root.addEventListener('change', function (e) {
            if (e.target && e.target.type === 'checkbox') {
                renderChips();

                var ids = selectedOptions().map(function (o) { return parseInt(inputOf(o).value, 10); });
                root.dispatchEvent(new CustomEvent('skillselector:change', { bubbles: true, detail: { ids: ids } }));
            }
        });

        document.addEventListener('click', function (e) {
            if (isOpen() && !root.contains(e.target)) close(false);
        });

        root.classList.add('is-enhanced');
        renderChips();
    }

    function initAll() {
        var roots = document.querySelectorAll('[data-skill-selector]');
        Array.prototype.forEach.call(roots, init);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAll);
    } else {
        initAll();
    }

    window.SkillSelector = { init: initAll };
})();
