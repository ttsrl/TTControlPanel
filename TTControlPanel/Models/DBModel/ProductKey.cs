using System;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class ProductKey
    {
        public enum PKType { Normal, Time, Trial }
        private string key;
        private DateTime? timestamp;

        public int Id { get; set; }
        public string Key { get => key; set => key = value.ToUpper(); }
        public User GenerateUser { get; set; }
        public PKType Type { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
