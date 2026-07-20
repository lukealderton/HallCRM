namespace CRM.Core.Common.Abstraction
{
    public interface IAppPathProvider
    {
        public string ContentRootPath { get; }
        public string WebRootPath { get; }
    }
}
