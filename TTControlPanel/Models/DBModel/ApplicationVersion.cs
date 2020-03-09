using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TTControlPanel.Models
{
    public class ApplicationVersion
    {
        private DateTime? releaseDate;
        private string v;

        public int Id { get; set; }
        public string Version { get => v; set => v = value; }
        [JsonIgnore]
        public Application Application { get; set; }
        public List<License> Licences { get; set; }
        public string Notes { get; set; }
        public DateTime ReleaseDate
        {
            get => releaseDate ?? DateTime.Now;
            set => releaseDate = value;
        }

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
