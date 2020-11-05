using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexClientGetModel
    {
        public List<ClientApps> Clients { get; set; } 
        public int Error { get; set; }

        public class ClientApps
        {
            public Client Client { get; set; }
            public List<License> Licenses { get; set; } 
        }
    }

    public class NewClientGetModel
    {
        public int Error { get; set; }
    }

    public class NewClientPostModel
    {
        public bool AutomaticCode { get; set; }
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Cap { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string VAT { get; set; }
    }

    public class EditClientGetModel
    {
        public Client Client { get; set; }
        public int Error { get; set; }
    }

    public class EditClientPostModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Cap { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string VAT { get; set; }
    }

    public class ClientDetailsGetMode
    {
        public Client Client { get; set; }
        public List<Application> Applications { get; set; }
        public List<License> Licenses { get; set; }
    }
}