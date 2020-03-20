using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Yandex.Disk.Api
{
    public class UrlBuilder
    {
        private readonly NamingConvention _namingConvention;

        public UrlBuilder(NamingConvention namingConvention)
        {
            _namingConvention = namingConvention;
        }
            
        public string Build(string baseUrl, string path, Dictionary<string, string> parameters)
        {
            string query = "";

            foreach (var (key, value) in parameters)
            {
                if (value == null)
                    continue;
                
                if (query != "")
                    query += "&";

                var key1 = _namingConvention == NamingConvention.SnakeCase ? key.ToSnakeCase() : key;
                
                query += key1 + "=" + HttpUtility.UrlEncode(value, Encoding.UTF8);
            }

            return baseUrl + "/" + path + "?" + query;
        }
    }
}