using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class HID
    {
        private DateTime? timestamp;

        public int Id { get; set; }
        public string Value { get; set; }

        public DateTime Timestamp
        {
            get => timestamp ?? DateTime.Now;
            set => timestamp = value;
        }
    }
}
