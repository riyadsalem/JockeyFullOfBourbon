using HorsesForCourses.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests.PersistenceTests;

// Gives every test its own fresh ... empty DB.
public abstract class PersistenceTestBase : IDisposable
{
    private readonly SqliteConnection connection; // The power cable
    private readonly DbContextOptions<AppDbContext> options; // The address slip
    protected AppDbContext Context { get; } // the main door

    protected PersistenceTestBase()
    {
        connection = new SqliteConnection("DataSource=:memory:"); // Get a brand new ... empty test tube...
        connection.Open(); // Plug the power innnnnn
        options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options; // Write down exactly which tube this
        Context = new AppDbContext(options); // Open the main door to it...
        Context.Database.EnsureCreated(); // Set up the shelves inside (the tables)
    }

    // Open a side door ((same tube... ))
    // Proves data was really saved... not just rememberd by the first door
    protected AppDbContext NewContext() => new(options);
    public void Dispose()
    {
        Context.Dispose(); // Close the main door 
        connection.Dispose(); // Cut the power ((The tube disappears...))
    }


}