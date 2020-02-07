using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexClientModel
    {
        public List<Client> Clients { get; set; }
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
}