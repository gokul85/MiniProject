namespace ReturnManagementSystem.Models
{
    public class ErrorModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorModel()
        {
            
        }

        public ErrorModel(int code, string message)
        {
            ErrorCode = code;
            ErrorMessage = message;
        }
    }
}
