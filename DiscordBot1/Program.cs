using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot1.Database;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot1
{
    public class Program
    {
        private const string CatSLmitSpielern = "Sl Spieler Chats";
        private const string SLRoleName = "SL";


        private readonly DiscordSocketClient _client;
        Dictionary<ulong, string> VampireUser;
        public static ulong VampireLiveGuildID { get; set; }

        public static ulong SLChatID = 99;


        private Random random = null;

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            //TestSQL2();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            random = new Random();
            VampireUser = new Dictionary<ulong, string>();
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            DiscordSocketConfig configdata = new DiscordSocketConfig();
            configdata.AlwaysDownloadUsers = true;
            _client = new DiscordSocketClient(configdata);
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.UserUpdated += UserUpdated;
            _client.GuildMemberUpdated += GuildUserUpdated;
            //_client.Dispose();
        }

        private async Task GuildUserUpdated(SocketGuildUser OldUser, SocketGuildUser NewUser)
        {
            Console.WriteLine($"GuildUserUpdated Username: {OldUser.Username} | Neuer Name: {NewUser.Username}");
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
            User userEntity = dataManager.GetSingle<User>(x => x.UserID == NewUser.Id);
            if (userEntity != null)
            {
                userEntity.Name = NewUser.Username;
                userEntity.NickName = NewUser.Nickname;
                var channel = _client.GetGuild(VampireLiveGuildID).GetTextChannel(userEntity.SLChannelID);
                if (channel != null && userEntity.GenerateChannelName() != channel.Name)
                {
                    await channel.ModifyAsync(x => x.Name = userEntity.GenerateChannelName());
                    Console.WriteLine($"Chanal wird umbenannt in:  {userEntity.GenerateChannelName()}");
                    userEntity.SLChannel = userEntity.GenerateChannelName();
                    bool ok = dataManager.Update<User>(userEntity);
                    if(!ok)
                    {
                        Console.WriteLine($"DBInsert hat nicht geklappt!");
                    }
                }
            }
        }

        private void TestAddChannel()
        {
            string channelName = "Test";

            Console.WriteLine($"Erzeuge neuen Kanal: {channelName}");
            try
            {
                var textchannel = _client.GetGuild(VampireLiveGuildID).CreateTextChannelAsync(channelName, x => { x.CategoryId = SLChatID; }).Result;

                //Lesen nur für den einen Spieler alle anderen unsichtbar setzten
                OverwritePermissions permissionsetEveryone = new OverwritePermissions(0, 105536);
                OverwritePermissions permissionset = new OverwritePermissions(105536, 0);
                //textchannel.AddPermissionOverwriteAsync(user, permissionset);
                textchannel.AddPermissionOverwriteAsync(_client.GetGuild(VampireLiveGuildID).EveryoneRole, permissionsetEveryone);

                Console.WriteLine($"Kanal: {textchannel.Name} wurde erzeugt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erzeuge neuen Kanal: {channelName} fehlgeschlagen mit fehler: {ex.Message}");
            }
        }

        /*private static void TestSQL()
        {
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);

            var result = dataManager.GetAll<Charakterblatt>();
            Charakterblatt blatt = new Charakterblatt();
            blatt.Name = "Stefan Cappenberg";

            CharakterWert wert = new CharakterWert();
            wert.name = "Kraft";
            wert.wert = 2;
            wert.character = blatt;
            blatt.AddWert(wert);
            bool ok = dataManager.Add<Charakterblatt>(blatt);
            if (!ok)
            {
                Console.WriteLine($"insert Wert faild");
            }

            wert = new CharakterWert();
            wert.name = "schnelligkeit";
            wert.wert = 3;
            wert.character = blatt;
            blatt.AddWert(wert);

            ok = dataManager.Update<Charakterblatt>(blatt);
            if (!ok)
            {
                Console.WriteLine($"insert Wert faild");
            }
        }

        
        private static void TestSQL2()
        {
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);

            var result = dataManager.GetAll<Charakterblatt>(x => x.charakterwertListe);

            foreach (Charakterblatt blatt in result)
            {
                var wert = new CharakterWert();
                wert.name = "schnelligkeit";
                wert.wert = 3;
                wert.characterID = blatt.ID;
                blatt.AddWert(wert);

                //bool ok = dataManager.Update<Charakterblatt>(blatt);
                bool ok = dataManager.Add<CharakterWert>(wert);
                if (!ok)
                {
                    Console.WriteLine($"insert Wert faild");

                }
            }
        }*/

        private async Task UserUpdated(SocketUser OldUser, SocketUser NewUser)
        {
            Console.WriteLine($"Alter Username: {OldUser.Username} | Neuer Name: {NewUser.Username}");
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
            User userEntity = dataManager.GetSingle<User>(x => x.UserID == NewUser.Id);
            if (userEntity != null)
            {
                SocketGuildUser guilduser = _client.GetGuild(VampireLiveGuildID).GetUser(NewUser.Id);
                SocketGuildUser guilduserCast = NewUser as SocketGuildUser;
                if(guilduserCast == null)
                    Console.WriteLine($"Guildusercast faild");
                userEntity.Name = guilduser.Username;
                userEntity.NickName = guilduser.Nickname;
                var channel = _client.GetGuild(VampireLiveGuildID).GetTextChannel(userEntity.SLChannelID);
                if (channel != null && userEntity.GenerateChannelName() != channel.Name)
                {
                    await channel.ModifyAsync(x => x.Name = userEntity.GenerateChannelName());
                    Console.WriteLine($"Chanal wird umbenannt in:  {userEntity.GenerateChannelName()}");
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            if(VampireUser.ContainsKey(user.Id))
            {
                VampireUser.Remove(user.Id);
                DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
                User userEntity = dataManager.GetSingle<User>(x => x.UserID == user.Id);
                if (userEntity != null)
                    dataManager.Delete<User>(userEntity);
            }
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            Console.WriteLine($"Neuer User auf dem Server: {user.Username}");

            AddVampireUser(user);

            /*
            if (!VampireUser.ContainsKey(user.Id))
            {
                if(user.Nickname == null)
                    VampireUser.Add(user.Id, user.Username);
                else
                    VampireUser.Add(user.Id, user.Nickname);


                User userEntity = new User();
                userEntity.UserID = user.Id;
                userEntity.NickName = user.Nickname;
                userEntity.Name = user.Username;

                DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);

                CreateSLChanIfNotExists(user, userEntity);

                bool ok = dataManager.Add<User>(userEntity);

            }*/
        }

        private async void CreateSLChanIfNotExists(SocketGuildUser user, User userEntity)
        {
            bool spieler = !user.IsBot;
            foreach (var role in user.Roles)
            {
                if (role.Name.Equals(SLRoleName))
                    spieler = false;
            }
            if (spieler)
            {
                //hier müssen evt mehr sonderzeichen ersetzt werden, Fallback währe den neuen Channel nochmal gegen die vorhandenen zu prüfen
                string channelName = userEntity.GenerateChannelName();
                var vampireGuild = _client.GetGuild(VampireLiveGuildID);
                var channel = ChannelExists(vampireGuild, channelName);
                if (channel == null)
                {
                    if (userEntity.SLChannel != null && !userEntity.SLChannel.Equals(""))
                    {
                        Console.WriteLine($"Umbenennen des alten Kanals : {userEntity.SLChannel}");
                        channel = _client.GetGuild(VampireLiveGuildID).GetTextChannel(userEntity.SLChannelID);
                        await channel.ModifyAsync(x => x.Name = channelName);
                    }
                    else
                    {
                        Console.WriteLine($"Erzeuge neuen Kanal: {channelName}");
                        try
                        {
                            var textchannel = _client.GetGuild(VampireLiveGuildID).CreateTextChannelAsync(channelName, x => { x.CategoryId = SLChatID; }).Result;

                            //Lesen nur für den einen Spieler alle anderen unsichtbar setzten
                            OverwritePermissions permissionsetEveryone = new OverwritePermissions(0, 105536);
                            OverwritePermissions permissionset = new OverwritePermissions(105536, 0);
                            await textchannel.AddPermissionOverwriteAsync(user, permissionset);
                            await textchannel.AddPermissionOverwriteAsync(_client.GetGuild(VampireLiveGuildID).EveryoneRole, permissionsetEveryone);

                            userEntity.SLChannel = textchannel.Name;
                            userEntity.SLChannelID = textchannel.Id;

                            Console.WriteLine($"Kanal: {textchannel.Name} wurde erzeugt");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erzeuge neuen Kanal: {channelName} fehlgeschlagen mit fehler: {ex.Message}");
                        }
                    }
                }
                else
                {
                    //fallback für channels die vor der umstellung erzeugt wurden.
                    userEntity.SLChannel = channelName;
                    userEntity.SLChannelID = channel.Id;
                }
            }
        }

        private SocketGuildChannel ChannelExists(SocketGuild vampireGuild, string channelName)
        {
            foreach(SocketGuildChannel channel in vampireGuild.Channels)
            {
                if (channel.Name.Equals(channelName))
                {
                    //Console.WriteLine($"Kanal {channelName} wurde gefunden.");
                    return channel;
                }
            }
            return null;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            StreamReader reader = new StreamReader("Token.txt");            
            String token = reader.ReadLine();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            //Analyse();
            Setup();
            return Task.CompletedTask;
        }

        private void Setup()
        {
            var guildList = _client.Guilds;
            Console.WriteLine($"Guilds:");
            foreach (SocketGuild guild in guildList)
            {
                Console.WriteLine($"{guild.Name}");
                if (guild.Name == "Vampire Live Düsseldorf")
                {
                    VampireLiveGuildID = guild.Id;
                }
            }
            SocketGuild vampireguild = _client.GetGuild(VampireLiveGuildID);
            if (!CategorieExits(vampireguild, CatSLmitSpielern))
            {
                var categorie = vampireguild.CreateCategoryChannelAsync(CatSLmitSpielern, x => { x.CategoryId = SLChatID; }).Result;
                SLChatID = categorie.Id;
                Console.WriteLine($"Categorie mit der ID {SLChatID} erstellt");
            }
        }

        private bool CategorieExits(SocketGuild vampireGuild, string categorie)
        {
            foreach (var channel in vampireGuild.Channels)
            {
                if (channel.Name.Equals(categorie))
                {
                    if (categorie.Equals(CatSLmitSpielern))
                    {
                        SLChatID = channel.Id;
                        Console.WriteLine($"Categorie {categorie} hat die ID {SLChatID}");
                    }
                    Console.WriteLine($"Categorie {categorie} wurde gefunden.");
                    return true;
                }
            }
            return false;
        }

        private void Analyse()
        {
            var guildList = _client.Guilds;
            Console.WriteLine($"Guilds:");
            IReadOnlyCollection<SocketGuildUser> usersVampire = null;
            IReadOnlyCollection<SocketGuildChannel> channelsVampire = null;
            SocketGuild guildVampire = null;
            foreach (var guild in guildList)
            {
                Console.WriteLine($"{guild.Name}");
                if (guild.Name == "Vampire Live Düsseldorf")
                {
                    VampireLiveGuildID = guild.Id;
                    guildVampire = guild;
                    usersVampire = guild.Users;
                    channelsVampire = guild.Channels;
                }
            }
            Console.WriteLine($"Guild Channels:");
            foreach (var channel in channelsVampire)
            {
                Console.WriteLine($"{channel.Name}");
            }
            Console.WriteLine($"Guild User:");
            foreach (var user in usersVampire)
            {
                Console.WriteLine($"{user.Username} / {user.Nickname} / {user.Id}");
                AddVampireUser(user);
            }
        }

        private void AddVampireUser(SocketGuildUser user)
        {
            try
            {

                if (VampireUser.ContainsKey(user.Id))
                    return;
                if (user.Nickname == null)
                {
                    VampireUser.Add(user.Id, user.Username);
                }
                else
                {
                    VampireUser.Add(user.Id, user.Nickname);
                }
                //Datenbank Entity anlegen

                DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
                User userEntity = dataManager.GetSingle<User>(x => x.UserID == user.Id);
                if (userEntity == null)
                {
                    userEntity = new User();
                    userEntity.UserID = user.Id;
                    userEntity.NickName = user.Nickname;
                    userEntity.Name = user.Username;
                    CreateSLChanIfNotExists(user, userEntity);

                    bool ok = dataManager.Add<User>(userEntity);
                }

                Console.WriteLine($"AddVampireUser: {user.Id} / {userEntity.GenerateChannelName()}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"AddVampireUser: {ex.Message}");
            }
            
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {

            //await _client.GetGuild(VampireLiveGuildID).DownloadUsersAsync();
            var userList = _client.GetGuild(VampireLiveGuildID).Users;
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            string lowerMessage = message.Content.ToLower();

            if (lowerMessage == "!ping")
                await message.Channel.SendMessageAsync("pong!");
            if (lowerMessage == "!serveranalyse")
                Analyse();
            if (lowerMessage == "!spielerchans")
                CreateSLChansAsync();

            if (message.Content.Length >= 6 && lowerMessage.StartsWith("!roll "))
                await Roll(message);

            if (lowerMessage == "!test")
                TestAddChannel();
            if (message.Content.Length >= 10 && lowerMessage.StartsWith("!addskill "))
                AddSkill(message);
            if (message.Content.Length >= 9 && lowerMessage.StartsWith("!setname "))
                SetName(message);
            if (lowerMessage == "!charakter")
                PrintCharakter(message);

        }

        private void PrintCharakter(SocketMessage message)
        {

            UserManager manager = new UserManager(message.Author.Id, _client);
            try
            { 
                manager.PrintCharakter();
            }
            catch (Exception ex)
            {
                message.Channel.SendMessageAsync(ex.Message);
            }
}

        private void SetName(SocketMessage message)
        {
            UserManager manager = new UserManager(message.Author.Id, _client);
            try
            {
                string parameter = message.Content.Substring(9).ToLower();

                manager.SetName(parameter);
            }
            catch (Exception ex)
            {
                message.Channel.SendMessageAsync(ex.Message);
            }
        }

        private void AddSkill(SocketMessage message)
        {
            UserManager manager = new UserManager(message.Author.Id, _client);

            string parameter = message.Content.Substring(10).ToLower();

            string[] split = parameter.Split(' ');
            try
            {
                string name = split[0];
                int wert = 0;
                Int32.TryParse(split[1], out wert);

                manager.AddUpdateWert(name, wert);


            }
            catch (Exception ex)
            {
                message.Channel.SendMessageAsync(ex.Message);
            }
        }

        private async Task Roll(SocketMessage message)
        {
            string username = message.Author.Username;

            string parameter = message.Content.Substring(6).ToLower();

            string[] split = parameter.Split('d');
            try
            {
                int count = 0;
                int result = 0;
                Int32.TryParse(split[0], out count);
                int dice = 0;
                Int32.TryParse(split[1], out dice);
                if (count <= 0)
                {
                    await message.Channel.SendMessageAsync($"Weniger als einen Würfel werfen macht wohl wenig Sinn... gib etwas größeres als {count} ein!");
                    return;
                }
                if (dice <= 0)
                {
                    await message.Channel.SendMessageAsync($"Was für ein Würfel soll das sein?... gib etwas größeres als {dice} ein!");
                    return;
                }
                string resultList= "";
                for (int index = 1; index <= count; index++)
                {
                    int diceroll = random.Next(1, dice);
                    resultList += ($"[{diceroll}]");
                    result += diceroll;
                }

                await message.Channel.SendMessageAsync($"{username} hat eine {result} geworfen");
                await message.Channel.SendMessageAsync(resultList);
            }
            catch(Exception ex)
            {
                await message.Channel.SendMessageAsync(ex.Message);
            }
        }

        private async Task CreateSLChansAsync()
        {
            try
            {
                var userList = _client.GetGuild(VampireLiveGuildID).Users;
                //var count = _client.GetGuild(VampireLiveGuildID).MemberCount;
                foreach (var user in userList)
                {
                    //Console.WriteLine($"Start Task AddVampireUser {user.Username}");
                    Task task = new Task(() => AddVampireUser(user));
                    task.Start();
                    //Console.WriteLine($"Ende Task AddVampireUser");
                    //AddVampireUser(user);                
                }
            }
            catch(Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
