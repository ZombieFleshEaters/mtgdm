using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace mtgdm.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SystemValue> SystemValue { get; set; }
        public virtual DbSet<Showpiece> Showpiece { get; set; }
        public virtual DbSet<ShowpieceRating> ShowpieceRating { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<ShowpieceToGenre> ShowpieceToGenre { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
    }
}
