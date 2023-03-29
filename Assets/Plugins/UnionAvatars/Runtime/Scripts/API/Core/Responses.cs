namespace UnionAvatars.API
{
    public class WebResponse
    {
        public bool success;
        public string responseErrorMessage;
    }

    public class WebResponse<T> : WebResponse
    {
        public T data;
    }
    public class ErrorResponse
    {
        public string detail;
    }
}