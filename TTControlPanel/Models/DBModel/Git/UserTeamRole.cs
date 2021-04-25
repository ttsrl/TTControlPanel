using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TTControlPanel.Models;

namespace TTGitServer.Models
{
    public class UserTeamRole : BaseEntity
    {
        public bool IsAdministrator { get; set; }
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
    }
}
