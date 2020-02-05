using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class ApplicationVersion
    {
        private DateTime? releaseDate;
        private string v;

        public int Id { get; set; }
        public string Version { get => v; set => v = value; }
        public List<License> Licences { get; set; }
        public DateTime ReleaseDate
        {
            get => releaseDate ?? DateTime.Now;
            set => releaseDate = value;
        }

        public Version GetVersion()
        {
            return System.Version.Parse(v);
        }
    }
}
