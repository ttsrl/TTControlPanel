using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TTControlPanel.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-checked")]
    public class CheckedTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-checked")] public bool If { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (If) output.Attributes.SetAttribute("checked", true);
        }
    }
}