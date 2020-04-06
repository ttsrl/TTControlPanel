using System;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class HID
    {
        private string val;
        private DateTime? timestamp;

        public int Id { get; set; }
        public string Value { get => val; set => val = value.ToUpper(); }
        public User AddedUser { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
