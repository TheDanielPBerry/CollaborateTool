namespace CollaborateDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserConversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDUserConversation { get; set; }

        public int? ConversationID { get; set; }

        public int? UserID { get; set; }

        public bool? IsOwner { get; set; }

        public virtual Conversation Conversation { get; set; }

        public virtual User User { get; set; }
    }
}
