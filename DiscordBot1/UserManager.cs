using Discord.WebSocket;
using DiscordBot1.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1
{
    public class UserManager
    {
        private readonly DiscordSocketClient Client;
        public ulong UserId { get; set; }
        public UserManager(ulong userId, DiscordSocketClient client)
        {
            UserId = userId;
            Client = client;
        }

        public void AddUpdateWert(string name, int wert)
        {
            if (wert <= 0)
                return;
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
            User userEntity = dataManager.GetSingle<User>(x =>  x.UserID == UserId, x => x.Charakter, x=> x.Charakter.charakterwertListe);
            if (userEntity != null)
            {

                var blatt = userEntity.Charakter;

                if(blatt == null)
                {
                    blatt = new Charakterblatt();
                    blatt.UserID = userEntity.ID;
                    blatt.Name = "Alric";
                    dataManager.Add<Charakterblatt>(blatt);
                    userEntity = dataManager.GetSingle<User>(x => x.UserID == UserId, x => x.Charakter, x => x.Charakter.charakterwertListe);
                    blatt = userEntity.Charakter;

                }
                var charakterWert = blatt.HasWert(name);
                if (charakterWert == null)
                {
                    charakterWert = new CharakterWert(name, wert);
                    charakterWert.characterID = blatt.ID;
                    dataManager.Add<CharakterWert>(charakterWert);
                }
                else
                {
                    charakterWert.wert = wert;
                    dataManager.Update<CharakterWert>(charakterWert);
                }
            }
        }

        public void PrintCharakter()
        {
            string charakterAusgabe = "";
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
            User userEntity = dataManager.GetSingle<User>(x => x.UserID == UserId, x => x.Charakter, x => x.Charakter.charakterwertListe);
            if (userEntity != null)
            {

                var blatt = userEntity.Charakter;
                if (blatt != null)
                {
                    if (blatt.charakterwertListe != null)
                    {
                        //erst den Namen
                        charakterAusgabe += $"Charaktername:  {blatt.Name} \n";
                        //dann alle werte
                        foreach (var wert in blatt.charakterwertListe)
                        {
                            charakterAusgabe += $"{wert.name} : \t  {wert.wert} \n";
                        }
                    }

                }
                var channel = Client.GetGuild(Program.VampireLiveGuildID).GetTextChannel(userEntity.SLChannelID);
                if (channel == null)
                    return;
                channel.SendMessageAsync(charakterAusgabe);
            }
        }

        public void SetName(string parameter)
        {
            DataManager<DBContextBot> dataManager = new DataManager<DBContextBot>(SystemContainer.DatabaseContextFactory);
            User userEntity = dataManager.GetSingle<User>(x => x.UserID == UserId, x => x.Charakter, x => x.Charakter.charakterwertListe);
            if (userEntity != null)
            {

                var blatt = userEntity.Charakter;
                if (blatt != null)
                {
                    blatt.Name = parameter;
                    dataManager.Update<Charakterblatt>(blatt);
                }
                else
                {
                    blatt = new Charakterblatt();
                    blatt.Name = parameter;
                    blatt.UserID = userEntity.ID;
                    dataManager.Add<Charakterblatt>(blatt);
                }
            }
        }
    }
}
