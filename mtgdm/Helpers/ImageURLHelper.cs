using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtgdm.Helpers
{
    public class ImageURLHelper
    {
        public static readonly int[] Heights = 
        {
            //1920,
            //1366,
            //1200,
            1100,
            1024,
            1000,
            900,
            800,
            737,
            700,
            640,
            //600,
            //548,
            //500,
            //489,
            //370
        };

        public static string GetImageURL(string url, int height)
        {
            return QueryHelpers.AddQueryString(url, new Dictionary<string, string>()
            {
                {"width", (height * 0.6666667).ToString()},
                {"height", height.ToString()}
            });
        }

        public static string GetStandardSizes(string url)
        {
            var sb = new StringBuilder();
            
            var sizes = new List<string>();
            foreach (var height in Heights)
            {
                var query = QueryHelpers.AddQueryString(url, new Dictionary<string, string>()
                {
                    {"width", (height * 0.6666667).ToString()},
                    {"height", height.ToString() }
                });

                sb.Append(query).Append(" ").Append(height).Append("w,");
            }
            sb.Length = sb.Length - 1; //Remove final comma
            return sb.ToString();
        }
    }
}
