using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class ShowpieceRating
    {
        [Key]
        public Guid ShowpieceRatingID { get; set; }

        public Guid ShowpieceID { get; set; }

        public Guid UserID { get; set; }

        public decimal Rating { get; set; }
    }
}
