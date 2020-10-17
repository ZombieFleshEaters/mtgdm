using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class Showpiece
    {
        [Key]
        public Guid ShowpieceID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; }

        public Guid UserID { get; set; }

        [DataType(DataType.ImageUrl)]
        [StringLength(255, MinimumLength = 3)]
        public string URL { get; set; }

        [Display(Name="Synopsis (optional)", Prompt ="Synopsis (optional)")]
        [StringLength(2000, MinimumLength = 3)]
        public string Synopsis { get; set; }

        public DateTime Created { get; set; }

        public bool Published { get; set; }

    }
}
