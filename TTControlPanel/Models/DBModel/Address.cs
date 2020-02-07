using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class Address
    {
        private string street = "";
        private string city = "";
        private string prov = "";
        public int Id { get; set; }
        public string Street { get => street; set { street = value?.ToTitleCase();  } }
        public string CAP { get; set; }
        public string City { get => city; set { city = value?.ToFirstCharUpper(); } }
        public string Province { get => prov; set { prov = value?.ToUpper(); } }
        public string Country { get; set; }
    }
}
