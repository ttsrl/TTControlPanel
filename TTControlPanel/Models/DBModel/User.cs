using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TTControlPanel.Utilities;
using TTGitServer.Models;

namespace TTControlPanel.Models
{
    public class User
    {

        private string _username = "";
        private string email = "";
        private string name = "";
        private string surname = "";
        private DateTime? timestamp;

        public int Id { get; set; }
        public bool Ban { get; set; }
        public string Username
        {
            get => _username;
            set => _username = value.ToLower().Replace(" ", "").Replace("'", "");
        }
        public string Name { get => name; set => name = value.ToTitleCase(); }
        public string Surname { get => surname; set => surname = value.ToTitleCase(); }
        public string Password { get; set; }
        public string Email { get => email; set => email = value.ToLower(); }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }

    }
}
