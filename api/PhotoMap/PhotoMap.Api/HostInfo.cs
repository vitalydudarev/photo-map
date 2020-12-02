using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace PhotoMap.Api
{
    public class HostInfo
    {
        public HostString Host { get; set; }
        public string Scheme { get; set; }

        public string GetUrl()
        {
            return UriHelper.BuildAbsolute(Scheme, Host);
        }
    }
}
