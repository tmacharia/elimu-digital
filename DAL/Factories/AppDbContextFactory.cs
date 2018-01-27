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

            builder.UseSqlServer("Server=TIMOTHY-PC;Database=le_pad_db;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new AppDbContext(builder.Options);
        }

    }
}
