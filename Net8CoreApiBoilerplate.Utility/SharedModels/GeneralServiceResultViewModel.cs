namespace Net8CoreApiBoilerplate.Utility.SharedModels
{
    public class GeneralServiceResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public GeneralServiceResultViewModel(bool isSuccess, string message = null)
        {
            Success = isSuccess;
            Message = message;
        }
    }
}
