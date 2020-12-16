using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1
{
    public static class SystemContainer
    {

        public static DatabaseContextFactory DatabaseContextFactory { get; private set; }

        static SystemContainer()
        {
            // TODO: Setup LoggerFactory

            DatabaseContextFactory = new DatabaseContextFactory();
        }

    }
}
