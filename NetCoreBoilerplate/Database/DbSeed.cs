using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NetCoreBoilerplate.Models.Database;

namespace NetCoreBoilerplate.Database
{
    public class DbSeed
    {
        public static void Initialize(ApplicationDbContext context)
        {
/*            if (!context.Database.EnsureCreated())
            {
                context.Database.Migrate();
            }*/
        }
    }
}