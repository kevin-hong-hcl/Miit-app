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
    public class BaseTest
    {
        protected DbContextOptions<DataContext> ContextOptions { get; }
        protected BaseTest(DbContextOptions<DataContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using (var context = new DataContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var user1 = new AppUser { Id = 1, UserName = "Hudson" };
                var user2 = new AppUser { Id = 2, UserName = "Kevin" };
                var user3 = new AppUser { Id = 3, UserName = "Matthew" };

                context.Users.AddRange(user1, user2, user3);

                context.SaveChanges();
            }
        }
    }
}
