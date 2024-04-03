namespace QuizBuilder.Models
{
    public class Result<T> : Result
    {
        public Result(T value) 
        {
            Value = value;
        }
        public Result(List<ErrorResult> errors) : base(errors) { }
        public Result(ErrorResult error) : base(error) { }
        public Result(int errorCode, string message) : base(errorCode, message) { }
        public T Value { get; set; }
    }

    public class Result 
    {
        public Result() { }
        public Result(List<ErrorResult> errors)
        {
            Errors = errors;
        }
        public Result(ErrorResult error)
        {
            Errors.Add(error);
        }
        public Result(int errorCode, string message)
        {
            Errors.Add(new ErrorResult(errorCode,message));
        }
        public bool Success => !Errors.Any();
        public List<ErrorResult> Errors { get; set; } = new List<ErrorResult>();
    }

    public class ErrorResult 
    {
        public ErrorResult(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
