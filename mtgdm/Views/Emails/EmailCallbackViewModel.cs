using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mtgdm.Views.Emails.Callback
{
    public class EmailCallbackViewModel
    {
        public EmailCallbackViewModel(string confirmEmailUrl)
        {
            ConfirmEmailUrl = confirmEmailUrl;
        }
        public string ConfirmEmailUrl { get; set; }
    }
}
