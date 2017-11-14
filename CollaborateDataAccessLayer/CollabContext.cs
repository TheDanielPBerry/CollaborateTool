namespace CollaborateDataAccessLayer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CollabContext : DbContext
    {
        public CollabContext()
            : base("Server=(localdb)\\v11.0;Database=TeamCollab;Trusted_Connection=True;")
        {
        }

        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<BoardColumn> BoardColumns { get; set; }
        public virtual DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Board>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<Board>()
                .HasMany(e => e.BoardColumns)
                .WithOptional(e => e.Board)
                .HasForeignKey(e => e.BoardID);

            modelBuilder.Entity<BoardColumn>()
                .HasMany(e => e.Cards)
                .WithOptional(e => e.BoardColumn)
                .HasForeignKey(e => e.ColumnID);

        }
    }
}
