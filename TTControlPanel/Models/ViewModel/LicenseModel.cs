﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexLicenseGetModel
    {
        public List<License> Licenses { get; set; }
        public int Error { get; set; }
        public List<LastLog> LastLogs { get; set; }
    }

    public class VersionLicensesGetModel
    {
        public ApplicationVersion ApplicationVersion { get; set; }
        public List<License> Licenses { get; set; }
        public int Error { get; set; }
        public List<LastLog> LastLogs { get; set; }
    }

    public class NewLicenseGetModel
    {
        public int Error { get; set; }
        public List<Application> Applications { get; set; }
        public List<Client> Clients { get; set; }
    }

    public class NewLicensePostModel
    {
        [Required]
        public int Application { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Days { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public int Client { get; set; }
        public string Notes { get; set; }
    }

    public class PrecompiledNewLicenseGetModel
    {
        public int Error { get; set; }
        public ApplicationVersion ApplicationVersion { get; set; }
        public List<Client> Clients { get; set; }
    }

    public class PrecompiledNewLicensePostModel
    {
        [Required]
        public int Type { get; set; }
        [Required]
        public int Days { get; set; }
        [Required]
        public int Client { get; set; }
        public string Notes { get; set; }
    }

    public class DetailsLicenseGetModel
    {
        public License License { get; set; }
        public int Error { get; set; }
    }

    public class RequestCodeGetModel
    {
        public License License { get; set; }
        public int Error { get; set; }
    }

    public class RequestCodePostModel
    {
        [Required]
        public string HID { get; set; }
    }
}
