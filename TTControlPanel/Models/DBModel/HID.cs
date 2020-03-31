using System;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class HID
    {
        private DateTime? timestamp;
        private string val;

        public int Id { get; set; }
        public string Value { get => val; set => val = value.ToUpper(); }
        public User AddedUser { get; set; }

        public DateTime Timestamp
        {
            get => timestamp ?? DateTimeCE.Now;
            set => timestamp = value;
        }
    }
}
