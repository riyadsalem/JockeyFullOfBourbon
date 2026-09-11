using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Courses.TimeSlots;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests.PersistenceTests;

public class CoursePersistenceTests : PersistenceTestBase
{
    [Fact]
    // Scenario (1) >> Save and reload a course....
    public async Task Course_CanBeSavedAndReloaded()
    {
        Course course = Course.Create("Advanced C#", new DateOnly(2026, 9, 11), new DateOnly(2026, 9, 21));
        Context.Courses.Add(course);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        Course reloaded = await freshContext.Courses.SingleAsync(c => c.Id == course.Id);

        Assert.Equal("Advanced C#", reloaded.Name.Value);
        Assert.Equal(new DateOnly(2026, 9, 11), reloaded.Period.Start);
        Assert.Equal(new DateOnly(2026, 9, 21), reloaded.Period.End);
        Assert.False(reloaded.IsConfirmed);
    }

    [Fact]
    // Scenario (2) >> required skills and timeslots round trip correctly....
    public async Task Course_RequiredSkillsAndTimeSlots_RoundTripCorrectly()
    {
        Course course = Course.Create("Advanced C#", new DateOnly(2026, 9, 11), new DateOnly(2026, 9, 21));
        course.UpdateRequiredSkills(["C#", "SQL"]);
        course.UpdateTimeSlots([(CourseDay.Monday, 9, 12), (CourseDay.Wednesday, 13, 17)], t => t);
        Context.Courses.Add(course);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        Course reloaded = await freshContext.Courses.SingleAsync(c => c.Id == course.Id);

        Assert.Equal(2, reloaded.RequiredSkills.Count);
        Assert.Equal(2, reloaded.TimeSlots.Count);
        Assert.Contains(reloaded.TimeSlots,
        t => t.Day == CourseDay.Monday
        && t.Start.Value == 9
        && t.End.Value == 12);
        Assert.Contains(reloaded.TimeSlots,
        t => t.Day == CourseDay.Wednesday
        && t.Start.Value == 13
        && t.End.Value == 17);
    }

    [Fact]
    /* Scenario (3 + 4) >> a confirmed course with an assigned coach...
    Reloaded with a consistent relationship in BOTH directions*/
    public async Task ConfirmedCourse_WithAssignedCoachReloads_WithAConsistentRelationship()
    {
        Coach coach = Coach.Create("Mark", "mark@jockeyfullofbourbon");
        coach.UpdateSkills(["C#"]);

        Course course = Course.Create("Advanced C#", new DateOnly(2026, 9, 11), new DateOnly(2026, 9, 21));
        course.UpdateRequiredSkills(["C#"]);
        course.UpdateTimeSlots([(CourseDay.Monday, 9, 12)], t => t);
        course.Confirm();
        course.AssignCoach(coach);

        Context.Coaches.Add(coach);
        Context.Courses.Add(course);
        await Context.SaveChangesAsync();

        var freshContext = NewContext();
        Course reloadedCourse = await freshContext.Courses
        .Include(c => c.AssignedCoach)
        .SingleAsync(c => c.Id == course.Id);
        Coach reloadedCoach = await freshContext.Coaches
        .Include(c => c.AssignedCourses)
        .SingleAsync(c => c.Id == coach.Id);

        Assert.True(reloadedCourse.IsConfirmed);
        Assert.NotNull(reloadedCourse.AssignedCoach);
        Assert.Equal(reloadedCoach.Id, reloadedCourse.AssignedCoach.Id);

        // both directions of the relationship must agree with each other.....
        Assert.Single(reloadedCoach.AssignedCourses);
        Assert.Equal(reloadedCourse.Id, reloadedCoach.AssignedCourses[0].Id);
    }

    [Fact]
    // Scenario (5) >> a loaded entity is changed through a real domain method... then saved again...
    public async Task ALoadedCourse_CanBeChanged_ThroughADomainMethod_AndSavedAgain()
    {
        Course course = Course.Create("Advanced C#", new DateOnly(2026, 9, 11), new DateOnly(2026, 9, 21));
        course.UpdateRequiredSkills(["C#"]);
        Context.Courses.Add(course);
        await Context.SaveChangesAsync();

        var eidtContext = NewContext();
        Course loaded = await eidtContext.Courses.SingleAsync(c => c.Id == course.Id);

        loaded.UpdateRequiredSkills(["C#", "SQL"]); // real domain method... same validation as always
        await eidtContext.SaveChangesAsync(); // The same door 

        var verifyContext = NewContext();
        Course reloaded = await verifyContext.Courses.SingleAsync(c => c.Id == course.Id);

        Assert.Equal(2, reloaded.RequiredSkills.Count);
    }
}