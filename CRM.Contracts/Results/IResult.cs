namespace CRM.Contracts.Results
{
    /// <summary>
    /// Defines a standard sync response/result format.
    /// </summary>
    public interface IResult
    {
        Boolean Success { get; set; }
        String Message { get; set; }
    }
}
