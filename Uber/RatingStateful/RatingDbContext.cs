﻿using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace RatingStateful;
public class RatingDbContext : DbContext
{
    public RatingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RatingModel> Ratings { get; set; }

}
