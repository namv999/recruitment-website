namespace recruitment_website.Services
{
    /* Kết quả của thao tác đổi trạng thái tin (Pause / Resume / Close). */
    public enum StatusChangeResult
    {
        Success,
        NotFound,      // tin không tồn tại hoặc không thuộc công ty của người thao tác
        InvalidState   // trạng thái hiện tại không cho phép thao tác này
    }
}
