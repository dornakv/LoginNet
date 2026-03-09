namespace LoginNet.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public Enum? ErrorCode { get; }

        protected Result(bool isSuccess, T? value, string? errorMessage, Enum? errorCode)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static Result<T> Success(T value) => new(true, value, null, null);
        public static Result<T> Failure(string errorMessage, Enum? errorCode = null) => new(false, default, errorMessage, errorCode);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public Enum? ErrorCode { get; }

        protected Result(bool isSuccess, string? errorMessage, Enum? errorCode)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static Result Success() => new(true, null, null);
        public static Result Failure(string errorMessage, Enum? errorCode = null) => new(false, errorMessage, errorCode);
    }
}
