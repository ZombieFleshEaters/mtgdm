#pragma checksum "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\UploadConfirm.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4150a22ead71b5b20b0e9bd3ab327672c97a19d5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(mtgdm.Pages.Pages_UploadConfirm), @"mvc.1.0.razor-page", @"/Pages/UploadConfirm.cshtml")]
namespace mtgdm.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\_ViewImports.cshtml"
using mtgdm;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\_ViewImports.cshtml"
using mtgdm.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4150a22ead71b5b20b0e9bd3ab327672c97a19d5", @"/Pages/UploadConfirm.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f41d12c213c22813ef6c3302e7c31b78dc7fde8", @"/Pages/_ViewImports.cshtml")]
    public class Pages_UploadConfirm : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\UploadConfirm.cshtml"
  
    var src = Model.Source;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<section class=\"section\">\r\n    <div class=\"container\">\r\n        <img");
            BeginWriteAttribute("src", " src=\"", 154, "\"", 164, 1);
#nullable restore
#line 10 "C:\Users\matad\source\repos\mtgdm\mtgdm\Pages\UploadConfirm.cshtml"
WriteAttributeValue("", 160, src, 160, 4, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n    </div>\r\n</section>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<mtdgm.Pages.UploadConfirmModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<mtdgm.Pages.UploadConfirmModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<mtdgm.Pages.UploadConfirmModel>)PageContext?.ViewData;
        public mtdgm.Pages.UploadConfirmModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591