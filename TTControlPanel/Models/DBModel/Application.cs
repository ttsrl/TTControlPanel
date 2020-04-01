using System.Collections.Generic;

namespace TTControlPanel.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ApplicationVersion> ApplicationVersions { get; set; }
    }
}
