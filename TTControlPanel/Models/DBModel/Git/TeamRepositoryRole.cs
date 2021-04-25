using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTGitServer.Models
{
    public class TeamRepositoryRole : BaseEntity
    {
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual Team Team { get; set; }
    }
}
