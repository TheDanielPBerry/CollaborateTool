using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CollaborateDAL
{
    class CollabContext : DbContext
        base("Server=(localdb)\\v11.0;Database=TeamCollab;Trusted_Connection=True;")
    {
        public DbSet<Board> boards { get; set; }
        public DbSet<Column> columns { get; set; }
        public DbSet<Board> boards { get; set; }
        public DbSet<Board> boards { get; set; }

    }
}
