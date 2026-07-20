namespace CRM.Contracts.Results
{
    public sealed class BasicResult : BasicResult<object> { }

    public class BasicResult<T> : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
    }
}
