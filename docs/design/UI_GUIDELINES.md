# HƯỚNG DẪN THIẾT KẾ VIEW LAYOUT (UI GUIDELINES)

> **Mục tiêu:** Hướng dẫn các thành viên trong nhóm cách dựng một Razor View chuẩn trong ASP.NET MVC, đảm bảo kế thừa hoàn hảo từ `_Layout.cshtml`, không đụng độ CSS/JS và có giao diện đồng bộ 100%.

---

## 1. Cấu Trúc Khung Chuẩn Của Một Razor View

Khi tạo một View mới (ví dụ `Index.cshtml`, `Details.cshtml`, `Create.cshtml`), hãy tuân thủ cấu trúc sau:

```cshtml
@model recruitment_website.Models.Job.JobIndexViewModel

@{
    ViewBag.Title = "Danh sách việc làm tuyển dụng";
}

@section styles {
    @* Nạp CSS riêng của module mình (nếu cần custom thêm) *@
    <link rel="stylesheet" href="~/Content/css/job/job.css" />
}

<!-- NỘI DUNG VIEW (ĐƯỢC ĐƯA VÀO @RenderBody() CỦA _LAYOUT.CSHTML) -->
<div class="container py-2">
    <!-- 1. Breadcrumb & Page Header -->
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb mb-1">
            <li class="breadcrumb-item"><a href="@Url.Action("Index", "Home")">Trang chủ</a></li>
            <li class="breadcrumb-item active" aria-current="page">Việc làm</li>
        </ol>
    </nav>

    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
            <h1 class="h3 fw-bold text-dark mb-1">Tìm kiếm việc làm IT</h1>
            <p class="text-muted small mb-0">Khám phá hơn 1,000+ cơ hội nghề nghiệp phù hợp</p>
        </div>
        <div>
            <!-- Action buttons nếu có (ví dụ: Tạo mới, Lọc nhanh) -->
            <a href="@Url.Action("Create", "Job")" class="btn btn-primary">
                + Đăng tin mới
            </a>
        </div>
    </div>

    <!-- 2. Thân nội dung (Bento Grid / Danh sách / Form / Bảng) -->
    <div class="row g-4">
        ...
    </div>
</div>

@section scripts {
    @* Nạp JS riêng của module mình *@
    <script src="~/Scripts/job/job.js"></script>
}
```

---

## 2. Quy Tắc Kế Thừa Layout & Sections

### 2.1. Không viết thêm thẻ `<html>`, `<head>`, `<body>`, `<main>`
File `Views/Shared/_Layout.cshtml` đã dựng sẵn:
- Thẻ `<header>` chứa Navbar dính (Sticky Header).
- Thẻ `<main class="main-content" id="mainContent">` bao bọc `@RenderBody()`.
- Thẻ `<footer>` đa cột ở chân trang.
- Container thông báo flash tự động (`TempData["Success"]`, `TempData["Error"]`).

👉 **Do đó trong file View con, bạn chỉ cần bắt đầu ngay bằng `<div class="container">` hoặc component nội dung.**

### 2.2. Khai báo Section Styles và Scripts
- **Style Section:** Khai báo ở đầu trang qua `@section styles { ... }`.
- **Script Section:** Khai báo ở cuối trang qua `@section scripts { ... }`.
- Luôn kiểm tra xem tính năng có thể giải quyết bằng Bootstrap 5 hay không trước khi viết thêm CSS/JS riêng.

---

## 3. Các Mô Hình Bố Cục Trang Phổ Biến

### 3.1. Bố Cục Trang Danh Sách Tuyển Dụng (Lọc bên trái + Kết quả bên phải)
Đây là màn hình kinh điển cho việc tìm kiếm việc làm, ứng viên:
```html
<div class="container py-3">
    <div class="row g-4">
        <!-- Cột Bộ Lọc (Filter Sidebar) -->
        <aside class="col-lg-3 col-md-4">
            <div class="bento-card p-3 sticky-top" style="top: 85px;">
                <h5 class="fw-bold mb-3 fs-6">Bộ lọc tìm kiếm</h5>
                <!-- Form lọc địa điểm, mức lương, kinh nghiệm... -->
            </div>
        </aside>

        <!-- Cột Danh Sách Kết Quả -->
        <section class="col-lg-9 col-md-8">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <span class="text-muted small">Tìm thấy <strong>48</strong> việc làm phù hợp</span>
                <!-- Dropdown sắp xếp -->
            </div>

            <div class="row g-3">
                <!-- Danh sách các thẻ Job Card -->
            </div>
        </section>
    </div>
</div>
```

### 3.2. Bố Cục Trang Bento Grid (Dashboard / Thống Kê / Tổng Quan)
Áp dụng cho trang Dashboard của Nhà tuyển dụng hoặc Admin:
```html
<div class="container py-3">
    <div class="row g-3">
        <!-- Thẻ thống kê 1 -->
        <div class="col-sm-6 col-lg-3">
            <div class="bento-card p-3">
                <div class="text-muted small mb-1">Tin đang đăng</div>
                <h3 class="fw-bold mb-0 text-primary">12</h3>
            </div>
        </div>
        <!-- Thẻ thống kê 2 -->
        <div class="col-sm-6 col-lg-3">
            <div class="bento-card p-3">
                <div class="text-muted small mb-1">Ứng viên mới nộp</div>
                <h3 class="fw-bold mb-0 text-success">38</h3>
            </div>
        </div>
        <!-- Thẻ thống kê 3 -->
        <div class="col-sm-6 col-lg-3">
            <div class="bento-card p-3">
                <div class="text-muted small mb-1">Lịch phỏng vấn</div>
                <h3 class="fw-bold mb-0 text-warning">5</h3>
            </div>
        </div>
        <!-- Thẻ thống kê 4 -->
        <div class="col-sm-6 col-lg-3">
            <div class="bento-card p-3">
                <div class="text-muted small mb-1">Lượt xem tin</div>
                <h3 class="fw-bold mb-0 text-secondary">1,420</h3>
            </div>
        </div>

        <!-- Khối Bento lớn bên dưới -->
        <div class="col-lg-8">
            <div class="bento-card p-4">
                <h5 class="fw-bold mb-3">Ứng viên mới ứng tuyển gần đây</h5>
                <!-- Bảng danh sách rút gọn -->
            </div>
        </div>
        <div class="col-lg-4">
            <div class="bento-card p-4">
                <h5 class="fw-bold mb-3">Thông báo tuyển dụng</h5>
                <!-- Hoạt động gần đây -->
            </div>
        </div>
    </div>
</div>
```

---

## 4. Hướng Dẫn Thiết Kế Form Nhập Liệu

Để form đẹp, hiện đại và không bị co méo:
1. **Chia nhóm rõ ràng:** Với các form dài như *Đăng tin tuyển dụng* hay *Cập nhật hồ sơ CV*, chia form thành các `bento-card` nhỏ (Thông tin chung, Yêu cầu kỹ năng, Quyền lợi, Thông tin liên hệ).
2. **Label rõ ràng:** Luôn đặt `class="form-label fw-semibold text-secondary"`.
3. **Hiển thị lỗi Validation:** Luôn đặt `@Html.ValidationMessageFor(...)` bên dưới mỗi trường nhập liệu với `class="text-danger small mt-1 d-block"`.
4. **Nút bấm:** Đặt ở cuối form, căn phải (`d-flex justify-content-end gap-2`). Nút submit là `btn-primary`, nút hủy là `btn-outline-secondary`.

---

## 5. Hướng Dẫn Thiết Kế Bảng Quản Lý Dữ Liệu (Tables)

Khi trình bày danh sách quản lý (Ứng viên, Tin đăng, Doanh nghiệp):
- Luôn bọc thẻ `<table>` trong `<div class="table-responsive">` để tránh vỡ giao diện trên di động.
- Dùng các class Bootstrap chuẩn: `table table-hover align-middle`.
- Cột trạng thái: dùng badge chuẩn (`.app-status-badge` hoặc `.badge`).
- Cột thao tác (Actions): gom các nút Xem, Sửa, Xóa về căn phải (`text-end`).

---

## 6. Trạng Thái Trống & Tải Dữ Liệu (Empty & Loading States)

Khi danh sách không có dữ liệu (ví dụ: *Chưa có hồ sơ nào được nộp*, *Không tìm thấy việc làm phù hợp*), **không để trang trống trơn**. Hãy sử dụng khối Empty State chuẩn:

```html
<div class="bento-card text-center py-5">
    <div class="mb-3 text-muted">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
        </svg>
    </div>
    <h5 class="fw-bold text-dark mb-1">Không tìm thấy kết quả phù hợp</h5>
    <p class="text-muted small mb-3">Hãy thử thay đổi từ khóa tìm kiếm hoặc bỏ bớt các tiêu chí lọc.</p>
    <a href="@Url.Action("Index", "Job")" class="btn btn-outline-primary btn-sm">Xóa bộ lọc</a>
</div>
```
