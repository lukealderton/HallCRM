namespace CRM.Primitives.Common.Abstraction
{
    /// <summary>
    /// Defines an item with an identifiable record in the application data database
    /// </summary>
    public interface IDbItem
    {
        /// <summary>
        /// The id of the item database record
        /// </summary>
        Guid Id { get; }
    }
}
