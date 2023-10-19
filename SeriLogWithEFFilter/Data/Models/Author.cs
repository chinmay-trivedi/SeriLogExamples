using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogWithEFFilter.Data.Models
{
    public class Author : IEntityBase
    {   
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorDescription { get; set; } = string.Empty;

    }
}
