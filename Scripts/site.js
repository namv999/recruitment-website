/**
 * Scripts/site.js
 * JavaScript dùng chung cho toàn bộ website tuyển dụng
 * Tuân thủ CONVENTIONS.md và .agents/skills/antigravity-design-expert/SKILL.md
 */

document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    // 1. Sticky Navbar Scroll Effect (add subtle shadow when scrolled)
    var navbar = document.querySelector('.navbar-site');
    if (navbar) {
        var handleScroll = function () {
            if (window.scrollY > 10) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        };
        window.addEventListener('scroll', handleScroll, { passive: true });
        handleScroll();
    }

    // 2. Auto-dismiss Flash Alerts
    var flashAlerts = document.querySelectorAll('.flash-alert[data-auto-dismiss="true"]');
    flashAlerts.forEach(function (alertElem) {
        setTimeout(function () {
            alertElem.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
            alertElem.style.opacity = '0';
            alertElem.style.transform = 'translateY(-6px)';
            setTimeout(function () {
                if (alertElem.parentNode) {
                    alertElem.parentNode.removeChild(alertElem);
                }
            }, 300);
        }, 5000);
    });

    // 3. Manual Dismiss for Alerts
    document.addEventListener('click', function (e) {
        var dismissBtn = e.target.closest('[data-dismiss="flash-alert"]');
        if (dismissBtn) {
            var alertBox = dismissBtn.closest('.flash-alert');
            if (alertBox) {
                alertBox.style.transition = 'opacity 0.2s ease, transform 0.2s ease';
                alertBox.style.opacity = '0';
                alertBox.style.transform = 'translateY(-6px)';
                setTimeout(function () {
                    if (alertBox.parentNode) {
                        alertBox.parentNode.removeChild(alertBox);
                    }
                }, 200);
            }
        }
    });

    // 4. Highlight Active Navigation Links based on URL
    var currentPath = window.location.pathname.toLowerCase();
    var navLinks = document.querySelectorAll('.navbar-nav .nav-link-site');
    navLinks.forEach(function (link) {
        var href = link.getAttribute('href');
        if (href) {
            var linkPath = href.toLowerCase();
            if ((currentPath === '/' && (linkPath === '/' || linkPath.endsWith('/home') || linkPath.endsWith('/home/index'))) ||
                (linkPath !== '/' && currentPath.indexOf(linkPath) === 0)) {
                link.classList.add('active');
            }
        }
    });

    // 5. Initialize Bootstrap Tooltips if Bootstrap JS is loaded
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});
