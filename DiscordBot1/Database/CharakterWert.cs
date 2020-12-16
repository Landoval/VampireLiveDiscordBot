using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1.Database
{
    public class CharakterWert : BaseEntity
    {
        public CharakterWert(string name, int wert)
        {
            this.name = name;
            this.wert = wert;
        }

        public string name { get; set; }
        public int wert { get; set; }

        [JsonIgnore]
        public virtual Charakterblatt character { get; set; }
        [JsonIgnore]
        public virtual Guid characterID { get; set; }
    }
}
