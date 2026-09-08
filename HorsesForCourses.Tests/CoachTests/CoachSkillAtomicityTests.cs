using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Skills;

namespace HorsesForCourses.Tests.CoachTests;

public class CoachSkillAtomicityTests : TestHelpers
{
    [Fact]
    public void UpdateSkills_With_AnInvalidSkill_DoesNotChangeExistingSkills()
    {
        Coach coach = CreateCoach(["C#"]);
        Assert.ThrowsAny<Exception>(() => coach.UpdateSkills(["SQL", ""]));
        Assert.Single(coach.Skills);
        Assert.Equal([Skill.From("C#")], coach.Skills);
    }
}