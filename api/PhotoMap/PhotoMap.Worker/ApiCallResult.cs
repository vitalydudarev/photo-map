namespace PhotoMap.Worker
{
    public class ApiCallResult<T>
    {
        public T Result { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
