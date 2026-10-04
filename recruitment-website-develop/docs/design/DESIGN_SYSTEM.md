# HỆ THỐNG THIẾT KẾ (DESIGN SYSTEM) — WEBSITE TUYỂN DỤNG RECRUITHUB

> **Dành cho các thành viên trong nhóm phát triển dự án LTW**  
> Tài liệu này là **nguồn quy chuẩn duy nhất (Single Source of Truth)** về phong cách thị giác, bảng màu, typography, khoảng cách và các quy tắc thiết kế chung cho toàn bộ website.

---

## 1. Triết lý Thiết kế Cốt lõi (Design Philosophy)

Website được xây dựng theo phong cách:  
👉 **Modern Minimalism + Bento Grid** *(Tối giản Hiện đại kết hợp Lưới Bento)*

### 5 Nguyên tắc vàng:
1. **Thông tin là ưu tiên số 1 (Content First):** Giao diện phải phục vụ cho việc đọc và quét nhanh thông tin tuyển dụng, CV, trạng thái ứng tuyển. Tránh các hiệu ứng trang trí thừa thãi gây phân tâm.
2. **Khoảng trắng có chủ đích (Spacious & Clean):** Sử dụng padding/margin rộng rãi, nhất quán để phân tách các khối nội dung thay vì lạm dụng quá nhiều đường kẻ (borders) hoặc bóng đổ (shadows) đậm.
3. **Màu sắc tiết chế và có ngữ nghĩa (Semantic Colors):** Màu xanh Royal (`#2563eb`) đại diện cho công nghệ, sự tin cậy. Các màu trạng thái (xanh lá, vàng cam, đỏ, tím) chỉ được dùng đúng mục đích ngữ nghĩa.
4. **Cấu trúc Bento Grid cho các khối tổng quan:** Áp dụng bố cục các ô thẻ (cards) bo góc nhẹ, viền mỏng, có phân cấp kích thước cho các trang Dashboard, Tổng quan tuyển dụng, Hồ sơ công ty và Thống kê.
5. **Đồng nhất công nghệ:** Sử dụng **Bootstrap 5.2.3** làm nền tảng layout/grid + **Custom CSS Tokens** trong `Content/css/site.css`. **Tuyệt đối không** dùng React, Tailwind CSS hay inline style lặp lại.

---

## 2. Hệ Màu Thiết Kế (Color Palette & Design Tokens)

Tất cả các màu đã được định nghĩa dưới dạng **CSS Custom Properties (Variables)** tại `Content/css/site.css`. Thành viên nhóm **bắt buộc sử dụng biến CSS hoặc class Bootstrap**, không gõ mã hex tùy tiện.

### 2.1. Màu Thương Hiệu & Điểm Nhấn (Brand Colors)
| Tên Biến CSS | Mã Hex | Xem trước | Ứng dụng |
|---|---|---|---|
| `--color-primary` | `#2563eb` | 🟦 Royal Blue | Nút bấm chính, liên kết, icon thương hiệu, trạng thái active |
| `--color-primary-hover` | `#1d4ed8` | 🟦 Darker Blue | Trạng thái hover của nút bấm/link chính |
| `--color-primary-light` | `#eff6ff` | ⬜ Ice Blue | Nền badge nhẹ, highlight hàng bảng, nền avatar |
| `--color-primary-border` | `#bfdbfe` | 🧊 Soft Border | Viền cho các khối được highlight hoặc active |

### 2.2. Màu Nền & Bề Mặt (Surfaces & Neutrals)
| Tên Biến CSS | Mã Hex | Mô tả |
|---|---|---|
| `--color-background` | `#f8fafc` | Nền canvas toàn bộ trang web (xám nhạt dịu mắt) |
| `--color-surface` | `#ffffff` | Nền trắng tinh khiết cho Card, Form, Navbar, Footer, Modal |
| `--color-surface-muted` | `#f1f5f9` | Nền xám cho ô phụ, thẻ tag kỹ năng, header bảng |

### 2.3. Màu Chữ (Typography Colors)
| Tên Biến CSS | Mã Hex | Mô tả |
|---|---|---|
| `--color-text` | `#0f172a` | Tiêu đề chính (H1-H6), chữ đậm, độ tương phản cao |
| `--color-text-secondary` | `#334155` | Nội dung văn bản chính, nhãn form (labels) |
| `--color-text-muted` | `#64748b` | Chữ phụ, ngày đăng, địa điểm, chú thích nhỏ |
| `--color-text-subtle` | `#94a3b8` | Placeholder, icon mờ, trạng thái vô hiệu |

### 2.4. Màu Ngữ Nghĩa Vòng Đời Tuyển Dụng (Recruitment Status Colors)
| Trạng thái | Màu sắc | Mã Hex | Ý nghĩa |
|---|---|---|---|
| **Applied** | Xanh dương | `#2563eb` | Ứng viên vừa nộp hồ sơ |
| **Reviewing** | Xanh da trời | `#0284c7` | Nhà tuyển dụng đang xem xét hồ sơ |
| **Interview** | Tím nhạt | `#7c3aed` | Đã lên lịch phỏng vấn |
| **Offer** | Cam ấm | `#d97706` | Đã gửi lời mời nhận việc |
| **Hired** | Xanh lá đậm | `#16a34a` | Tuyển dụng thành công |
| **Rejected** | Đỏ | `#dc2626` | Hồ sơ không phù hợp / Đã từ chối |

---

## 3. Hệ Thống Kiểu Chữ (Typography)

- **Font Family chính:** `'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif`
- Đã được nạp sẵn từ Google Fonts tại thẻ `<head>` của `_Layout.cshtml`.

### Bảng phân cấp Font Size & Weight:
| Cấp bậc | Font Size | Line Height | Font Weight | Class khuyên dùng |
|---|---|---|---|---|
| **Page Title** | 2rem (32px) | 1.25 | 700 / 800 | `h2` hoặc `display-6 fw-bold` |
| **Section Title** | 1.5rem (24px) | 1.3 | 700 | `h3 fw-bold` |
| **Card / Component Title** | 1.125rem (18px) | 1.35 | 600 / 700 | `h5 fw-bold` hoặc `.job-card-title` |
| **Body (Văn bản thường)** | 0.95rem (15.2px) | 1.55 | 400 | Thẻ `<p>` mặc định |
| **Small / Metadata** | 0.85rem (13.6px) | 1.4 | 500 | `small` hoặc `text-muted` |
| **Badge / Tag** | 0.75rem (12px) | 1.2 | 600 / 700 | `.badge` hoặc `.job-skill-pill` |

---

## 4. Hệ Thống Bo Góc & Bóng Đổ (Radii & Elevation)

### Bo góc (Border Radius):
- **Small (`6px`):** Nút nhỏ, input form, dropdown item (`--radius-sm`).
- **Medium (`10px`):** Nút tiêu chuẩn, alert box, logo công ty (`--radius-md`).
- **Large (`16px`):** Bento card, job card, auth card, modal (`--radius-lg` hoặc `rounded-4`).
- **Pill (`9999px`):** Badges trạng thái, tag tìm kiếm (`--radius-full` hoặc `rounded-pill`).

### Bóng đổ (Shadows):
- **Subtle (Nhẹ):** `box-shadow: 0 1px 2px rgba(15, 23, 42, 0.05);` → dùng cho navbar, input focus.
- **Card (Mặc định):** `box-shadow: 0 2px 8px -2px rgba(15, 23, 42, 0.06);` → dùng cho các thẻ Bento.
- **Card Hover:** `box-shadow: 0 12px 24px -4px rgba(15, 23, 42, 0.08); transform: translateY(-2px);` → phản hồi rê chuột mượt mà.

---

## 5. Quy Chuẩn Đồ Họa & Icon (Iconography)

- Sử dụng **SVG trực tiếp (Stroke: 2px hoặc 2.2px, bo tròn round)** hoặc **Bootstrap Icons**.
- Kích thước icon chuẩn:
  - Icon trong nút bấm: `16x16px`
  - Icon metadata (ngày, địa điểm, mức lương): `16x16px` hoặc `18x18px`
  - Icon trong thẻ tính năng / badge: `24x24px` đến `32x32px`

---

## 6. Những điều NÊN LÀM và KHÔNG ĐƯỢC LÀM (Do's & Don'ts)

### ✅ NÊN LÀM:
- Sử dụng thẻ ngữ nghĩa HTML5: `<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<footer>`.
- Luôn sử dụng hệ thống cột của Bootstrap (`col-12 col-md-6 col-lg-4...`) để giao diện tự co giãn tốt trên điện thoại.
- Đặt tiền tố class CSS theo đúng module của mình (ví dụ `.job-`, `.candidate-`, `.company-`).
- Kế thừa layout chung qua `@RenderBody()` và chỉ khai báo CSS/JS riêng qua `@section styles` và `@section scripts`.

### ❌ KHÔNG ĐƯỢC LÀM:
- **Không** viết inline style lặp lại (`style="color: blue; padding: 20px;"`).
- **Không** viết đè lên các class Bootstrap gốc (`.btn`, `.card`, `.form-control`) trong file CSS module của mình.
- **Không** thêm framework CSS khác (Tailwind, AntDesign, v.v.).
- **Không** tự ý bọc thêm thẻ `<main>` bên trong các View con vì `_Layout.cshtml` đã có sẵn `<main id="mainContent">`.
