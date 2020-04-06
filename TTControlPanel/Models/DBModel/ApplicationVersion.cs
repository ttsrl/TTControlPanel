using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class ApplicationVersion
    {
        private string v;
        private DateTime? timestamp;

        public int Id { get; set; }
        public string Version { get => v; set => v = value; }
        [JsonIgnore]
        public Application Application { get; set; }
        public List<License> Licences { get; set; }
        public string Notes { get; set; }
        public User AddedUser { get; set; }
        public DateTime ReleaseDateTimeUtc { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }

        public Version GetVersion()
        {
            Version o;
            if (System.Version.TryParse(v, out o))
                return o;
            else
                return null;
        }
    }
}
