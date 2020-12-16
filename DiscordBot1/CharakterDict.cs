using DiscordBot1.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1
{
    public class CharakterDict : Dictionary<int, Charakterblatt>
    {
        public bool Load()
        {
            return true;
        }

    }
}
