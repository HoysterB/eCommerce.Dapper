namespace eCommerce.Domain.Models.Responses.Default
{
    public class Response<T> where T : class
    {
        public dynamic Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}