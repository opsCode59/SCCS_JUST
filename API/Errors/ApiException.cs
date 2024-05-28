namespace API.Errors
    {
    public class ApiException
        {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public ApiException(int StatusCode, string Message , string Details ) {
         
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Details = Details;
         }
        }
    }
