using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HorsesForCourses.Persistence;

/*
AppDbContextFactory >>> HorsesForCourses is a plain class library NOOO ((Program.cs))
The ((Dotnet ef)) CLI needs something that knows how to construct an ((AppDbContext))
to generate igrations... sinc there is no running application to ask...
AppDbContextFactory implementing ((IDesignTimeDbContextFactory<AppDbContext>))
fills that role... used onnly by the CLI at design time >> It plays no part in the 
actual application or tests....
*/

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite("Data Source=design_time_placeholder.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}