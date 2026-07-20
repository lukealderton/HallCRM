using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;

namespace CRM.Core.Logging.Domain
{
    /// <summary>
    /// Represents an action or event that has been recorded.
    /// </summary>
    public sealed class Log
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public LogType LogType { get; set; }
        public LogArea LogArea { get; set; }
        public Guid? MemberId { get; set; }
        public Guid? RelId { get; set; }
        public ItemType RelType { get; set; }
        public String? Text { get; set; }

        public T? GetObjectFromText<T>()
        {
            if (String.IsNullOrWhiteSpace(Text))
            {
                return default;
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(Text);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
