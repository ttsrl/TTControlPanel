using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public enum LoginError
    {
        None,
        UsernameEmail,
        Password,
        Banned
    }

    public class LoginModel
    {
        public LoginError Error { get; set; }
    }
}
