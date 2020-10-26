using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class Report
    {
        [Key]
        public Guid ReportID { get; set; }
        
        public string Source { get; set; }

        public Guid SourceID { get; set; }

        [Display(Name = "Comment", Prompt = "Comment")]
        [Required]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Comments must be at least 20 characters")]
        public string Comment { get; set; }

        public Guid ReportedBy { get; set; }

        public DateTime Reported { get; set; }

        public Guid ResolvedBy { get; set; }

        public DateTime Resolved { get; set; }

    }
}
