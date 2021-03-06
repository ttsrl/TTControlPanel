﻿using System;
using TTControlPanel.Utilities;

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
        public User AddedUser { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
