namespace MessageApi.Models
{
    public class ResponseBody<T>
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public T? Data { get; set; }
    }
}
