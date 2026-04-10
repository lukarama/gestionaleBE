namespace Gestionale.Api.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int? StatusCode { get; set; }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> Created(T data)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                StatusCode = 201
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}