using System;
using Final.Models;
using Microsoft.EntityFrameworkCore;

namespace Final.Data;

    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }

