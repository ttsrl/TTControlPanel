using System;
using System.Collections.Generic;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class Working
    {
        private DateTime? timestamp;

        public int Id { get; set; }
        public Order Order { get; set; }
        public Client FinalClient { get; set; }
        public List<WorkingItem> Items { get; set; }
        public bool AtProgress { get; set; }
        public DateTime? StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
