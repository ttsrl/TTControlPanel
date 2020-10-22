using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TTControlPanel.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-disabled")]
    public class DisabledTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-disabled")] public bool If { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (If) output.Attributes.SetAttribute("disabled", "disabled");
        }
    }
}