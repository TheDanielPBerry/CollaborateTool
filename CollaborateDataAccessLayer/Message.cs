namespace CollaborateDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IDMessage { get; set; }

        [Column("Message")]
        [StringLength(511)]
        public string Message1 { get; set; }

        public DateTime Created { get; set; }

        public int ConversationID { get; set; }

        public int UserID { get; set; }

        public virtual Conversation Conversation { get; set; }

        public virtual User User { get; set; }
    }
}
