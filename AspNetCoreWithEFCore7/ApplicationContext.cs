using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWithEFCore7;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем бд с новой схемой
    }
}