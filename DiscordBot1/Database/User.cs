using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1.Database
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public ulong UserID { get; set; }
        public string SLChannel { get; set; }
        public ulong SLChannelID { get; set; }
        public virtual Guid CharakterID { get; set; }
        public virtual Charakterblatt Charakter { get; set; }

        public string GenerateChannelName()
        {
            string channeName = "";
            if (NickName == null || NickName.Equals(""))
                channeName = Name;
            else
                channeName = NickName;

            return "sl-mit-" + channeName.ToLower().Replace(" ", "-").Replace("(", "").Replace(")", "").Replace("/", "").Replace(".", "").Replace("|", "").Replace("--", "-").Replace("---", "-");
        }
    }
}
