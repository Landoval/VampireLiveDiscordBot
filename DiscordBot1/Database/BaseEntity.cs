using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot1.Database
{
    public class BaseEntity
    {
        [Key]
        [Column(Order = 0)]
        public Guid ID { get; set; }

        [Required]
        public string UserCreated { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        //[DateTimePrecision(0)]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string UserModified { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        //[DateTimePrecision(0)]
        public DateTime ModifiedOn { get; set; }

        public string ExtensionClass { get; set; }

        public BaseEntity()
        {
            this.ID = Guid.NewGuid();

            var now = DateTime.Now;


            this.UserModified = "???";
            this.UserCreated = "???";

            this.CreatedOn = now;
            this.ModifiedOn = now;

            this.ExtensionClass = null;
        }


        public virtual bool GetDeleteFlag()
        {
            return false;
        }
    }
}
