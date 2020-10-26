using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mtgdm.Helpers;

namespace mtgdm.Views.Shared
{
    public class ResponsiveImageViewModel
    {
        public ResponsiveImageViewModel(string url, string title, int defaultHeight)
        {
            Url = url;
            ImageSources = ImageURLHelper.GetStandardSizes(url);
            ImageDefault = ImageURLHelper.GetImageURL(url, defaultHeight);
            Title = title;
        }

        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageSources { get; set; }
        public string ImageDefault { get; set; }
    }
}
