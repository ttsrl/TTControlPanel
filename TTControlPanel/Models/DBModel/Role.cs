using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TTControlPanel.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /*------------------------ VIEW PAGE GRANTS ------------------------*/
        public bool GrantLogin { get; set; }

        public bool GrantUseCPanel { get; set; }
        public bool GrantUseGit { get; set; }

        [NotMapped]
        public bool this[string grant]
        {
            get => GetGrant(grant);
            set => SetGrant(grant, value);
        }

        private bool GetGrant(string grant)
        {
            var p = GetType().GetProperty(grant);
            if (p == null) return false;
            return (bool)p.GetValue(this, null);
        }

        private void SetGrant(string grant, bool value)
        {
            var p = GetType().GetProperty(grant);
            if (p != null) p.SetValue(this, value);
        }

        public static IEnumerable<string> GetGrantNames()
        {
            return from p in typeof(Role).GetProperties() where p.PropertyType == typeof(bool) && p.Name != "Item" && p.Name != "IsStaff" select p.Name;
        }
    }
}