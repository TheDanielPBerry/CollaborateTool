namespace CollaborateDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Board")]
    public partial class Board
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Board()
        {
            BoardColumns = new HashSet<BoardColumn>();
        }

        [Key]
        public int IDBoard { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(15)]
        public string Color { get; set; }

        public int? Owner { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BoardColumn> BoardColumns { get; set; }
    }
}
