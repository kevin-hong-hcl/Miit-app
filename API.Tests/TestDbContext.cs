using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests
{
    public class TestDbContext : DataContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        { 
        }
        public new virtual DbSet<AppUser> Users { get; set; }
        public new virtual DbSet<Photo> Photos { get; set; }


    }
}
