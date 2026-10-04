using System;
using recruitment_website.Constants;

namespace recruitment_website.Models.Job
{
    /* Một dòng trong bảng "Quản lý tin đã đăng" (Job/MyJobs). View dùng List<JobManageItemViewModel>. */
    public class JobManageItemViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public short Headcount { get; set; }
        public int ViewCount { get; set; }
        public int ApplicationCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string StatusLabel { get { return JobStatus.Label(Status); } }

        // Quyết định nút nào hiện ở View (View không tự so sánh chuỗi trạng thái)
        public bool CanResume { get { return Status == JobStatus.Paused; } }
        public bool CanPause { get { return Status == JobStatus.Published; } }
        public bool CanClose { get { return Status == JobStatus.Published || Status == JobStatus.Paused || Status == JobStatus.Pending; } }
        public bool CanEdit { get { return JobStatus.IsEditable(Status); } }
    }
}
