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

            //builder.UseSqlServer("Server=tcp:uczaep277a.database.windows.net,1433;Database=le_pad_db;User ID=phoneguru;Password=304MPS203#;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=True");
            builder.UseSqlServer("Server=.;Database=le_pad_db;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new LePadContext(builder.Options);
        }
        public LePadContext Create()
        {
            DbContextOptionsBuilder<LePadContext> builder = new DbContextOptionsBuilder<LePadContext>();

            //builder.UseSqlServer("Server=.;Database=le_pad_db;Trusted_Connection=True;MultipleActiveResultSets=true");
            builder.UseSqlServer("Server=tcp:uczaep277a.database.windows.net,1433;Database=le_pad_new;User ID=phoneguru;Password=304MPS203#;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=True");

            return new LePadContext(builder.Options);
        }
    }
}
