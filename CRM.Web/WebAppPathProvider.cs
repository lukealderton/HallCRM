using CRM.Core.Common.Abstraction;

namespace CRM.Web
{
    public sealed class WebAppPathProvider : IAppPathProvider
    {
        private readonly IWebHostEnvironment _objEnv;

        public WebAppPathProvider(IWebHostEnvironment objEnv)
        {
            _objEnv = objEnv;
        }

        public string ContentRootPath => _objEnv.ContentRootPath;
        public string WebRootPath => _objEnv.WebRootPath;
    }
}
