using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogWithEFFilter.Data.Models
{
    public class Book : IEntityBase
    {
        public string BookName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

    }
}
