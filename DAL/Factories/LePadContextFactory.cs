using DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Factories
{
    public class LePadContextFactory : IDesignTimeDbContextFactory<LePadContext>
    {
        public LePadContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<LePadContext> builder = new DbContextOptionsBuilder<LePadContext>();
            builder.UseSqlServer("");

            return new LePadContext(builder.Options);
        }
    }
}
