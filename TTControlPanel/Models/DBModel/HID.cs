using System;

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
            get => timestamp ?? DateTime.Now;
            set => timestamp = value;
        }
    }
}
