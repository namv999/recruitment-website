# THƯ VIỆN COMPONENT CHUẨN (COMPONENTS LIBRARY)

> **Dành cho các thành viên trong nhóm:**  
> Dưới đây là các đoạn mã mẫu HTML/Razor chuẩn đã được tối ưu hóa theo Bootstrap 5 và `site.css`. Bạn có thể sao chép (copy) và dán (paste) trực tiếp vào View của mình để đảm bảo giao diện toàn website đẹp và đồng nhất.

---

## 1. Thẻ Bento Cơ Bản & Thống Kê (Bento Cards & Stat Cards)

### 1.1. Thẻ Bento tiêu chuẩn
```html
<div class="bento-card">
    <div class="bento-card-header">
        <h5 class="bento-card-title">Tiêu đề khối</h5>
        <span class="badge bg-light text-secondary border">Thông tin phụ</span>
    </div>
    <p class="text-secondary small mb-0">Nội dung chi tiết bên trong thẻ bento.</p>
</div>
```

### 1.2. Thẻ số liệu thống kê (Metric / Stat Card)
```html
<div class="bento-card p-3">
    <div class="d-flex justify-content-between align-items-center mb-2">
        <span class="text-muted small fw-semibold">Tổng số hồ sơ</span>
        <div class="rounded-circle bg-primary-subtle text-primary p-2 d-flex">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
                <polyline points="14 2 14 8 20 8"></polyline>
            </svg>
        </div>
    </div>
    <h3 class="fw-bold mb-1 text-dark">142</h3>
    <span class="text-success small fw-semibold">↑ +12% so với tháng trước</span>
</div>
```

---

## 2. Thẻ Việc Làm (Job Card) — Module Job

Áp dụng cho các trang danh sách việc làm (`Views/Job/Index.cshtml`, `Views/Home/Index.cshtml`):

```html
<div class="job-card">
    <div class="d-flex align-items-start gap-3 mb-3">
        <!-- Logo công ty -->
        <img src="~/Content/images/company-placeholder.png" alt="Company Logo" class="company-logo" style="width: 52px; height: 52px;" />
        
        <div class="flex-grow-1">
            <h5 class="job-card-title mb-1">
                <a href="@Url.Action("Details", "Job", new { id = 1 })">Senior ASP.NET Core & MVC Developer</a>
            </h5>
            <div class="job-company-name mb-1">Công ty Cổ phần Công nghệ FPT</div>
            <div class="d-flex align-items-center gap-3 text-muted small flex-wrap">
                <span>📍 Cầu Giấy, Hà Nội</span>
                <span>💼 Toàn thời gian</span>
                <span>🕒 2 ngày trước</span>
            </div>
        </div>
    </div>

    <!-- Tags kỹ năng & Mức lương -->
    <div class="d-flex justify-content-between align-items-center mt-auto pt-3 border-top border-light flex-wrap gap-2">
        <div class="d-flex gap-1 flex-wrap">
            <span class="job-skill-pill">C#</span>
            <span class="job-skill-pill">ASP.NET MVC</span>
            <span class="job-skill-pill">SQL Server</span>
        </div>
        <span class="job-salary-tag">25 - 35 triệu</span>
    </div>
</div>
```

---

## 3. Badges Trạng Thái Ứng Tuyển (Application Status Badges) — Module Application

Bắt buộc sử dụng đúng class tương ứng với CHECK constraint trạng thái trong cơ sở dữ liệu:

```html
<!-- 1. Đã nộp (applied) -->
<span class="app-status-badge app-status-applied">
    ● Đã nộp hồ sơ
</span>

<!-- 2. Đang duyệt (reviewing) -->
<span class="app-status-badge app-status-reviewing">
    ● Đang xem xét
</span>

<!-- 3. Phỏng vấn (interview) -->
<span class="app-status-badge app-status-interview">
    ● Mời phỏng vấn
</span>

<!-- 4. Đề nghị nhận việc (offer) -->
<span class="app-status-badge app-status-offer">
    ● Đã gửi offer
</span>

<!-- 5. Trúng tuyển (hired) -->
<span class="app-status-badge app-status-hired">
    ✔ Trúng tuyển
</span>

<!-- 6. Bị từ chối (rejected) -->
<span class="app-status-badge app-status-rejected">
    ✖ Không phù hợp
</span>
```

---

## 4. Thẻ Ứng Viên (Candidate Card) — Module Candidate

Áp dụng cho danh sách ứng viên mà nhà tuyển dụng xem:

```html
<div class="candidate-card">
    <div class="d-flex align-items-center gap-3 mb-3">
        <img src="~/Content/images/avatar-default.png" alt="Candidate Avatar" class="candidate-avatar" style="width: 56px; height: 56px;" />
        <div>
            <h5 class="fw-bold mb-0 text-dark">Nguyễn Văn An</h5>
            <div class="candidate-headline">Lập trình viên Backend .NET | 3 năm kinh nghiệm</div>
            <span class="text-muted small">📍 TP. Hồ Chí Minh</span>
        </div>
    </div>

    <div class="mb-3">
        <div class="text-muted small mb-1 fw-semibold">Kỹ năng nổi bật:</div>
        <div class="d-flex gap-1 flex-wrap">
            <span class="badge bg-light text-secondary border">C#</span>
            <span class="badge bg-light text-secondary border">Entity Framework</span>
            <span class="badge bg-light text-secondary border">Web API</span>
        </div>
    </div>

    <div class="d-flex justify-content-end gap-2 pt-2 border-top">
        <a href="@Url.Action("Details", "Candidate", new { id = 1 })" class="btn btn-outline-secondary btn-sm">Xem hồ sơ</a>
        <a href="mailto:an.nguyen@example.com" class="btn btn-primary btn-sm">Liên hệ ứng viên</a>
    </div>
</div>
```

---

## 5. Nút Bấm Chuẩn (Buttons)

```html
<!-- Nút Hành động Chính (Primary Action) -->
<button type="submit" class="btn btn-primary">
    <span>Lưu thông tin</span>
</button>

<!-- Nút Viền Phụ (Secondary / Outline) -->
<a href="@Url.Action("Index", "Job")" class="btn btn-outline-secondary">
    <span>Quay lại</span>
</a>

<!-- Nút Đăng Tin Nổi Bật (CTA Button) -->
<a href="@Url.Action("Create", "Job")" class="btn btn-cta-post-job">
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
    <span>Đăng tin tuyển dụng</span>
</a>

<!-- Nút Hành động Nguy hiểm (Xóa / Hủy) -->
<button type="button" class="btn btn-outline-danger btn-sm" onclick="confirmDelete(1)">
    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
    <span>Xóa tin</span>
</button>
```

---

## 6. Khung Thông Báo Thành Công Có Badge (Success Alert with Badge)

Dùng cho các thông báo xác nhận quan trọng (như sau khi nộp CV thành công, đăng ký tài khoản thành công):

```html
<div class="alert alert-success d-flex align-items-start gap-3 mb-4 p-3 rounded-3 shadow-sm border border-success border-opacity-25 bg-success bg-opacity-10 text-success-emphasis" role="alert">
    <div class="rounded-circle bg-success text-white d-flex align-items-center justify-content-center flex-shrink-0 mt-1" style="width: 28px; height: 28px;">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
            <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
    </div>
    <div class="flex-grow-1">
        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
            <span class="badge bg-success text-white fw-bold px-2 py-1 rounded-pill" style="font-size: 0.725rem;">Thành công</span>
            <strong class="text-success" style="font-size: 0.95rem;">Thao tác hoàn tất!</strong>
        </div>
        <div class="text-dark opacity-75 small">Hồ sơ ứng tuyển của bạn đã được chuyển đến nhà tuyển dụng.</div>
    </div>
</div>
```

---

## 7. Phân Trang Chuẩn (Pagination)

Dành cho các trang danh sách việc làm / ứng viên có phân trang:

```html
<nav aria-label="Page navigation" class="mt-4">
    <ul class="pagination justify-content-center">
        <li class="page-item disabled">
            <a class="page-link" href="#" tabindex="-1" aria-disabled="true">&laquo; Trang trước</a>
        </li>
        <li class="page-item active" aria-current="page">
            <span class="page-link">1</span>
        </li>
        <li class="page-item"><a class="page-link" href="#">2</a></li>
        <li class="page-item"><a class="page-link" href="#">3</a></li>
        <li class="page-item">
            <a class="page-link" href="#">Trang sau &raquo;</a>
        </li>
    </ul>
</nav>
```
