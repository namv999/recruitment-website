# Quy ước chung khi code — Website Quản lý Tin Tuyển dụng Nhân sự

File này bổ sung cho `TEAM_SETUP.md`. Mục tiêu: 4 người code song song mà không đụng file, không đoán tên biến/route của nhau, review PR nhanh hơn.

## 1. Đặt tên C#

| Đối tượng | Quy tắc | Ví dụ |
|---|---|---|
| Class, Controller, Action | PascalCase | `JobController`, `public ActionResult Details(int id)` |
| Interface | Bắt đầu bằng `I` | `IApplicationService` |
| Biến local, tham số | camelCase | `jobId`, `candidateProfile` |
| Field private | `_camelCase` | `private readonly recruitment_dbEntities _db;` |
| Property (public) | PascalCase | `public string FullName { get; set; }` |
| Hằng số / string trạng thái | PascalCase, gom vào 1 class `Constants` | `ApplicationStatus.Applied` thay vì gõ tay `"applied"` |

**Bắt buộc:** không gõ tay chuỗi trạng thái (`"applied"`, `"published"`, `"pending"`...) rải rác trong code. Tạo 1 file `Constants/AppConstants.cs` chứa các class tĩnh tương ứng CHECK constraint trong DB (`JobStatus`, `ApplicationStatus`, `UserRole`...). Ai đụng tới trạng thái nào thì dùng hằng số đó — tránh gõ sai chính tả (`"Published"` vs `"published"`) gây lỗi filter WHERE.

## 2. ViewModel (thư mục `Models/`)

Đặt tên theo mẫu: `{Feature}{Action}ViewModel`
Models/ chỉ chứa ViewModel tự viết.
EF Entity không được đặt trong Models/.
EF Entity nằm trong DAL/ do EDMX sinh ra.

```
Models/
  Job/
    JobCreateViewModel.cs
    JobSearchViewModel.cs
    JobDetailViewModel.cs
  Candidate/
    CandidateProfileViewModel.cs
  Application/
    ApplicationStatusViewModel.cs
```

Mỗi người chỉ tạo file trong thư mục con của module mình (`Models/Job/`, `Models/Candidate/`...) — không tạo chung 1 file `ViewModels.cs` để tránh conflict merge liên tục.

## 3. Logic riêng cho Entity (EDMX partial class)

Không sửa file `.Designer.cs`. Tạo file riêng cùng namespace `DAL`, đặt trong `DAL/Partial/`:

```
DAL/Partial/Job.Partial.cs        → partial class Job { ... }
DAL/Partial/Candidate.Partial.cs  → partial class Candidate { ... }
```

## 4. Route / URL

Dùng route mặc định `{controller}/{action}/{id}` (đã có sẵn trong `RouteConfig.cs`, **không tự thêm route riêng** — nếu cần route đặc biệt (vd slug SEO cho job `/job/lap-trinh-vien-aspnet-mvc-fresher`) thì báo D vì đây là file dùng chung.

Action đặt tên đúng ngữ nghĩa REST-ish, không tự sáng tác:
- Danh sách: `Index`
- Xem chi tiết: `Details(int id)`
- Tạo mới: `Create` (GET hiển thị form, POST xử lý — overload cùng tên khác `[HttpPost]`)
- Sửa: `Edit(int id)`
- Đổi trạng thái (không phải sửa toàn bộ): action riêng, ví dụ `Pause(int id)`, `Close(int id)`, `ChangeStatus(int applicationId, string newStatus)`

## 5. Content / Scripts — chia theo module, không dùng chung 1 file

```
Content/
  css/
    site.css              ← style chung toàn site (không sửa)
    auth/
      auth.css
    company/
      company.css
    job/
      job.css
    candidate/
      candidate.css
    application/
      application.css
Scripts/
  site.js                 ← JS chung (không sửa)
  auth/
    auth.js
  company/
    company.js
  job/
    job.js
    job-search.js         ← tách riêng nếu file search/filter quá dài
  candidate/
    candidate.js
    cv-upload.js
  application/
    application.js
```

Quy tắc:
- **Không** viết CSS global đè lên class Bootstrap mặc định (`.btn`, `.card`...) trong file module — nếu cần custom style cho 1 thành phần, prefix class theo module: `.job-card`, `.candidate-avatar`, `.app-status-badge`. Tránh trường hợp CSS module B vô tình đổi giao diện màn hình module C.
- Mỗi View chỉ `@Scripts.Render` / link đúng file JS/CSS của module mình trong `_Layout.cshtml` (dùng `Section` để mỗi View tự khai báo CSS/JS riêng thay vì load hết mọi file JS ở mọi trang) — phần khai báo `Section` trong `_Layout` là do D dựng sẵn, hỏi D cú pháp cụ thể khi Layout xong.

## 6. Partial View dùng chung (`Views/Shared/`)

Prefix `_` + PascalCase, ví dụ `_JobCard.cshtml`, `_SkillSelector.cshtml`, `_Pagination.cshtml`.

Trước khi tự viết 1 component UI lặp lại (card hiển thị job, dropdown chọn skill, phân trang...), **kiểm tra `Views/Shared/` xem đã có ai làm chưa** — đây là chỗ dễ trùng công sức nhất giữa B và C (cả 2 đều cần UI chọn skill).

## 7. Service layer (nếu tách logic khỏi Controller)

```
Services/
  IJobService.cs / JobService.cs
  IApplicationService.cs / ApplicationService.cs
```
Không bắt buộc cho MVP nếu thời gian gấp — Controller gọi thẳng `DbContext` cũng được, nhưng nếu tách thì đặt đúng cấu trúc trên, không lẫn vào `Controllers/`.

### Controller:

Controller:
- nhận request
- validate
- gọi Service/DbContext
- chuẩn bị ViewModel
- return View/Redirect

Không nên chứa:
- business logic dài
- thuật toán matching
- xử lý trạng thái phức tạp
- logic tính toán nhiều bước

## 8. Trước khi tạo Controller/View mới

Chạy `git pull` trước, kiểm tra branch `develop` xem người khác đã tạo Controller cùng tên hay Route trùng chưa (đặc biệt B/C vì cùng đụng `Views/Shared/` và `Models/`).

## 9. Check nhanh trước khi PR

- [ ] Build sạch, không warning đỏ
- [ ] Không sửa file thuộc nhóm "chỉ 1 người sửa" (xem `TEAM_SETUP.md` mục 4) trừ khi đã báo trước
- [ ] Không gõ tay string trạng thái — dùng `Constants`
- [ ] CSS/JS đặt đúng thư mục module, không viết đè `site.css`/`site.js`
- [ ] ViewModel đặt đúng `Models/{Module}/`

### Git Commit

Format:

<type>: <description>

type:
feat     - chức năng mới
fix      - sửa bug
refactor - thay đổi cấu trúc code
style    - CSS/UI/format
docs     - tài liệu
chore    - cấu hình/setup
test     - test

Ví dụ:
feat: add job CRUD
feat: add candidate profile
fix: validate application status
style: update job listing UI
docs: update team conventions
chore: setup connection string

## 10. Razor View

Views/
  Job/
    Index.cshtml
    Details.cshtml
    Create.cshtml
    Edit.cshtml

  Candidate/
    Profile.cshtml
    Edit.cshtml

  Application/
    Index.cshtml
    Details.cshtml

- Tên View phải khớp với Action.
- Không viết business logic trong Razor.
- Không truy vấn DbContext trực tiếp từ View.
- View nhận ViewModel và chỉ chịu trách nhiệm hiển thị.

## 11. Web API

API Controller đặt trong:
Controllers/Api/

Tên:
JobsApiController
ApplicationsApiController
CandidatesApiController

Endpoint dùng HTTP verb đúng ngữ nghĩa:
GET    → lấy dữ liệu
POST   → tạo
PUT    → cập nhật
DELETE → xoá

Không trả EF Entity trực tiếp nếu có nguy cơ expose dữ liệu không cần thiết.
Ưu tiên DTO/ViewModel cho API response.
API route/config không tự ý sửa; nếu cần thay đổi route dùng chung, trao đổi với Nam trước.