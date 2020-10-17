using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mtgdm.Views.Shared
{
    public class EmailButtonViewModel
    {
        public EmailButtonViewModel(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; set; }
        public string Url { get; set; }
    }
}
