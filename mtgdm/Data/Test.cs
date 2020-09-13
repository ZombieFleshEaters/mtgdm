using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class Test
    {
        [Key]
        public Guid TestID { get; set; }
    }
}
