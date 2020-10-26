using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class AnalyticsSummary
    {
        [Key]
        public Guid AnalyticsSummaryID { get; set; }

        public DateTime Created { get; set; }

        public long RowCount { get; set; }
    }
}
