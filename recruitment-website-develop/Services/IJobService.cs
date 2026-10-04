using System.Collections.Generic;
using recruitment_website.Models.Job;

namespace recruitment_website.Services
{
    public interface IJobService
    {
        /// <summary>
        /// Tìm kiếm + lọc tin công khai. Đọc điều kiện từ model, ghi kết quả (Jobs, TotalItems, Page)
        /// và dữ liệu dropdown (CategoryOptions, CityOptions) ngược vào chính model đó.
        /// </summary>
        void Search(JobSearchViewModel model);

        /// <summary>
        /// Lấy chi tiết 1 tin. Trả về null nếu không tồn tại hoặc người xem không có quyền xem
        /// (tin chưa công khai / đã hết hạn chỉ chủ tin hoặc Admin xem được).
        /// Tự tăng view_count khi khách hoặc ứng viên xem tin đang công khai.
        /// </summary>
        JobDetailViewModel GetDetail(long id, long? currentUserId, string currentUserRole);

        /// <summary>company_id của công ty mà user đang thuộc về (null nếu chưa thuộc công ty nào).</summary>
        long? GetCompanyId(long userId);

        bool CategoryExists(int categoryId);

        /// <summary>Nạp dữ liệu dropdown ngành nghề + danh sách kỹ năng cho form Đăng/Sửa tin.</summary>
        void LoadFormOptions(JobFormViewModel model);

        /// <summary>
        /// Tạo tin mới với trạng thái Pending (chờ Admin duyệt). Trả về id tin vừa tạo.
        /// Ném InvalidOperationException nếu user chưa thuộc công ty nào.
        /// </summary>
        long Create(JobCreateViewModel model, long userId);

        /// <summary>
        /// Nạp form sửa tin (kèm dropdown). Trả về null nếu tin không tồn tại hoặc user không có quyền quản lý tin này.
        /// </summary>
        JobEditViewModel GetForEdit(long id, long userId);

        /// <summary>
        /// Lưu thay đổi. Trả về trạng thái mới của tin, hoặc null nếu không tìm thấy / không có quyền / tin không còn sửa được.
        /// </summary>
        string Update(JobEditViewModel model, long userId);

        /// <summary>Danh sách tin của công ty mình (mọi trạng thái), mới nhất trước.</summary>
        List<JobManageItemViewModel> GetMyJobs(long userId);

        /// <summary>published -> paused</summary>
        StatusChangeResult Pause(long id, long userId);

        /// <summary>paused -> published (tin đã được duyệt trước đó nên không cần duyệt lại)</summary>
        StatusChangeResult Resume(long id, long userId);

        /// <summary>draft / pending / published / paused -> closed</summary>
        StatusChangeResult Close(long id, long userId);
    }
}
