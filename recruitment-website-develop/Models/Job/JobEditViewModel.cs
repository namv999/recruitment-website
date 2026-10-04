using recruitment_website.Constants;

namespace recruitment_website.Models.Job
{
    /* Form sửa tin. Id và CurrentStatus được giữ ở hidden field / nạp lại từ DB, không cho người dùng tự đổi trạng thái. */
    public class JobEditViewModel : JobFormViewModel
    {
        public long Id { get; set; }

        public string CurrentStatus { get; set; }

        /// <summary>True nếu lưu thay đổi sẽ đưa tin về trạng thái chờ duyệt lại (để View hiện cảnh báo).</summary>
        public bool WillBeReviewedAgain { get; set; }

        public string CurrentStatusLabel { get { return JobStatus.Label(CurrentStatus); } }
    }
}
