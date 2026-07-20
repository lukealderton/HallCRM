namespace CRM.Core.Common.Domain
{
    public class SearchResult<T>
    {
        public SearchResult()
        {
            Items = [];
        }
        public Int32 TotalCount { get; set; }
        public List<T> Items { get; set; }
    }
}
