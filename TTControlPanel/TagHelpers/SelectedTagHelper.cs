using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TTControlPanel.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-selected")]
    public class SelectedTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-selected")] public bool If { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (If) output.Attributes.SetAttribute("selected", "selected");
        }
    }
}