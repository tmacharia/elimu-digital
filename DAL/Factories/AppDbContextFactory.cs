using DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Factories
{
    public class AppDbContextFactory : IDbContextFactory<AppDbContext>
    {
        public AppDbContext Create(DbContextFactoryOptions options)
        {
            DbContextOptionsBuilder<AppDbContext> builder = new DbContextOptionsBuilder<AppDbContext>();

            builder.UseSqlServer("");
            //builder.UseSqlServer("");

            return new AppDbContext(builder.Options);
        }

    }
}
