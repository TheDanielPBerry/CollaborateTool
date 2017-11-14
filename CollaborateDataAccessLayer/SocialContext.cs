namespace CollaborateDataAccessLayer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SocialContext : DbContext
    {
        public SocialContext()
            : base("data source=(localdb)\\v11.0;initial catalog=TeamCollab;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<UserBoard> UserBoards { get; set; }
        public virtual DbSet<UserConversation> UserConversations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conversation>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.Conversation)
                .HasForeignKey(e => e.ConversationID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Conversation>()
                .HasMany(e => e.UserConversations)
                .WithOptional(e => e.Conversation)
                .HasForeignKey(e => e.ConversationID);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<User>()
                .HasMany(e => e.UserBoards)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.UserID);


            modelBuilder.Entity<User>()
                .HasMany(e => e.UserConversations)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.UserID);
        }
    }
}
