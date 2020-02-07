using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class Client
    {
        private string vat = "";
        private DateTime? timestamp;

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public string VAT { get => vat; set { vat = value.ToUpper(); } }
        public List<Application> Applications { get; set; }
        public DateTime Timestamp 
        {
            get => timestamp ?? DateTime.Now;
            set => timestamp = value;
        }
    }
}
