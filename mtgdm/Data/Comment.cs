using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mtgdm.Data
{
    public class Comment
    {
        [Key]
        public Guid CommentID { get; set; }
        public Guid ShowpieceID { get; set; }
        public Guid UserID { get; set; }

        [Display(Name = "Comment", Prompt = "Comment")]
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Comments must be at least 3 characters")]
        public string Content { get; set; }
        public DateTime Created { get; set; }
    }
}
