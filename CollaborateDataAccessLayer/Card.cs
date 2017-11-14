namespace CollaborateDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Card
    {
        [Key]
        public int IDCard { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(3000)]
        public string Description { get; set; }

        public int? ColumnID { get; set; }

        public int? Points { get; set; }

        public virtual BoardColumn BoardColumn { get; set; }
    }
}
