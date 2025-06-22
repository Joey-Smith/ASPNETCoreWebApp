namespace WebApplicationMVC.Models.ViewModels
{
    public static class StatusType
    {
        public const string Success = "success";
        public const string Error = "error";
    }

    public class StatusMessageViewModel
    {
        public string Message { get; set; }
        public string Status { get; set; }

        public StatusMessageViewModel(string message, string status)
        {
            Message = message;
            Status = status;
        }
    }
}
