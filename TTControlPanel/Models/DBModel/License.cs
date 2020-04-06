using System;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class License
    {
        private string confirmCode;
        private DateTime? timestamp;

        public int Id { get; set; }
        public ProductKey ProductKey { get; set; }
        public ApplicationVersion ApplicationVersion { get; set; }
        public HID Hid { get; set; }
        public string ConfirmCode { get => confirmCode; set => confirmCode = value.ToUpper(); }
        public bool Active { get; set; }
        public bool Banned { get; set; }
        public DateTime? ActivationDateTimeUtc { get; set; }
        public Client Client { get; set; }
        public string Notes { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }

    }
}
