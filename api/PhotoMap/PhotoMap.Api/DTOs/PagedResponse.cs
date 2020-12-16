namespace PhotoMap.Api.DTOs
{
    public class PagedResponse<T>
    {
        public T[] Values { get; set; }

        public int Total { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
