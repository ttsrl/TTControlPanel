using System;

namespace TTControlPanel.Models
{
    public enum Api { ActiveLicense, GetLicense, UpdateDateTimeLicense }

    public class LastLog
    {
        public int Id { get; set; }
        public License License { get; set; }
        public Api Api { get; set; }
        public DateTime? DateTimeUtc { get; set; }
    }
}
