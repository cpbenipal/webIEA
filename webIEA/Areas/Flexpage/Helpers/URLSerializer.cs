using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FlexPage.Helpers
{
    public class URLSerializer
    {
        private static IDictionary<string, string> ToKeyValue(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            JToken token = obj as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(obj));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = ToKeyValue(child);
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                                                 .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", CultureInfo.InvariantCulture) :
                            jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }

        public static FormUrlEncodedContent Serialize(object obj)
        {
            var pairs = ToKeyValue(obj);
            return new FormUrlEncodedContent(pairs);
        }
    }
}