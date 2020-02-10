using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexLicenseModel
    {
        public List<License> Licenses { get; set; }
    }

    public class VersionLicensesModel
    {
        public ApplicationVersion ApplicationVersion { get; set; }
        public List<License> Licenses { get; set; }
    }

    public class NewLicenseGetModel
    {
        public int Error { get; set; }
        public List<Application> Applications { get; set; }
        public List<Client> Clients { get; set; }
        public ApplicationVersion Selected { get; set; }
    }

    public class NewLicensePostModel
    {
        [Required]
        public int Application { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public int Client { get; set; }
    }

    public class DetailsLicenseModel
    {
        public License License { get; set; }
    }

    public class ConfirmCodeGetModel
    {
        public License License { get; set; }
        public int Error { get; set; }
    }

    public class ConfirmCodePostModel
    {
        [Required]
        public string HID { get; set; }
    }
}
