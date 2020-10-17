using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace mtgdm.Data
{
    public class ShowpieceToGenre
    {

        [Key]
        public Guid ShowpieceToGenreID { get; set; }

        public Guid ShowpieceID { get; set; }
        public Guid GenreID { get; set; }

    }
}
