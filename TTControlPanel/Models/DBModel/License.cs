using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class License
    {
        private DateTime? releaseDate;

        public int Id { get; set; }
        public ProductKey ProductKey { get; set; }
        public ApplicationVersion ApplicationVersion { get; set; }
        public string HID { get; set; }
        public string ConfirmCode { get; set; }
        public bool Activate { get; set; }
        public DateTime? ActivateDateTime { get; set; }
        public Client Client { get; set; }
        public DateTime ReleaseDate
        {
            get => releaseDate ?? DateTime.Now;
            set => releaseDate = value;
        }
    }
}
