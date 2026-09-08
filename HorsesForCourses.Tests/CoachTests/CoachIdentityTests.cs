using HorsesForCourses.Domain.Coaches;

namespace HorsesForCourses.Tests.CoachTests;

public class CoachIdentityTests : TestHelpers
{
    [Fact]
    public void TwoCoaches_With_TheSameNameAndEmail_AreEqual()
    {
        Coach coach1 = Coach.Create("Mark", "mark@jokeyfullofbourbon");
        Coach coach2 = Coach.Create("Mark", "mark@jokeyfullofbourbon");
        Assert.Equal(coach1, coach2);
    }

    [Fact]
    public void TowCoaches_With_DifferentEmails_AreNotEqual()
    {
        Coach coach1 = Coach.Create("Mark", "mark@jokeyfullofbourbon");
        Coach coach2 = Coach.Create("Mark", "mark2@jokeyfullofbourbon");
        Assert.NotEqual(coach1, coach2);
    }
}