using System.ComponentModel.DataAnnotations.Schema;

namespace todo.Models
{
    public abstract class Auditable
    {
        [Column("date_created")]
        public DateTimeOffset DateCreated { get; set; }
        [Column("date_updated")]
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
