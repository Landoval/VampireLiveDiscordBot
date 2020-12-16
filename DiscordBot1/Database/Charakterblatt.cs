using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1.Database
{
    public class Charakterblatt : BaseEntity
    {
        public string Name { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }

        public virtual IList<CharakterWert> charakterwertListe { get; set; }

        public void AddWert(CharakterWert wert)
        {
            if(charakterwertListe==null)
            {
                charakterwertListe = new List<CharakterWert>();
            }
            charakterwertListe.Add(wert);
        }

        public CharakterWert HasWert(string name)
        {
            if (charakterwertListe == null)
                return null;
            foreach(var wert in charakterwertListe)
            {
                if (wert.name.Equals(name))
                    return wert;
            }
            return null;
        }
    }
}
