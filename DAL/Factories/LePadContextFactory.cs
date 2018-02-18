using DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Factories
{
    public class LePadContextFactory : IDbContextFactory<LePadContext>
    {
        public LePadContext Create(DbContextFactoryOptions options)
        {
            DbContextOptionsBuilder<LePadContext> builder = new DbContextOptionsBuilder<LePadContext>();

            //builder.UseSqlServer("");
            builder.UseSqlServer("");

            return new LePadContext(builder.Options);
        }
    }
}
