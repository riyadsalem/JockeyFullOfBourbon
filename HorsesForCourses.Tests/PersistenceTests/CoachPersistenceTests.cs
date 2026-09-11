using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Skills;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests.PersistenceTests;

public class CoachPersistenceTests : PersistenceTestBase
{
    [Fact]
    // Scenario (1) >> Save and reload a coach....
    public async Task Coach_CanBeSavedAndReloaded()
    {
        Coach coach = Coach.Create("Mark", "mark@jokeyfullofbourbon.com");
        Context.Coaches.Add(coach);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        Coach reloaded = await freshContext.Coaches.SingleAsync(c => c.Id == coach.Id);

        Assert.Equal("Mark", reloaded.Name.Value);
        Assert.Equal("mark@jokeyfullofbourbon.com", reloaded.Email.Value);
    }

    [Fact]
    // Senario (2) >>> Skills round trip correctly....
    public async Task Coach_SkillsRoundTripCorrectly()
    {
        Coach coach = Coach.Create("Mark", "mark@jokeyfullofbourbon.com");
        coach.UpdateSkills(["C#", "SQL"]);
        Context.Coaches.Add(coach);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        Coach reloaded = await freshContext.Coaches.SingleAsync(c => c.Id == coach.Id);

        Assert.Equal(2, reloaded.Skills.Count);
        Assert.Contains(Skill.From("C#"), reloaded.Skills);
        Assert.Contains(Skill.From("SQL"), reloaded.Skills);
    }

    [Fact]
    // Senario (6) >>> two coaches whith identical name/email but different Ids....
    public async Task MultipleCoaches_WithSameNameAndEmail_AreSavedAsSeparateEntities()
    {
        Coach coach1 = Coach.Create("Mark", "mark@jokeyfullofbourbon.com");
        Coach coach2 = Coach.Create("Mark", "mark@jokeyfullofbourbon.com");
        Context.Coaches.AddRange(coach1, coach2);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        List<Coach> all = await freshContext.Coaches.ToListAsync();

        Assert.Equal(2, all.Count);
        Assert.NotEqual(all[0].Id, all[1].Id);

    }
}