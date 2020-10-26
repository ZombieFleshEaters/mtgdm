using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class AnalyticsDetail
    {
        [Key]
        public Guid AnalyticsDetailID { get; set; }

        public Guid AnalyticsSummaryID { get; set; }

        public string PagePath { get; set; }

        public long Visitors { get; set; }

        public long Views { get; set; }
    }
}
