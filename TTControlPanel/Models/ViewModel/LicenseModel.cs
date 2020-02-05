using System;
using System.Collections.Generic;
using System.Linq;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexLicenseModel
    {
        public ApplicationVersion ApplicationVersion { get; set; }
        public List<License> Licenses { get; set; }
    }
}
