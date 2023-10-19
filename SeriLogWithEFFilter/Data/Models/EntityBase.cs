using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogWithEFFilter.Data.Models
{
    public class EntityBase
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }

    }
}
