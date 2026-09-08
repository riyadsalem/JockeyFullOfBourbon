using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Coaches.InvalidationReasons;
using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Skills;

namespace HorsesForCourses.Tests.CoachTests;

public class CoachSkillUpdateInvariantTests : TestHelpers
{
    [Fact]
    public void UpdatingSkills_To_NoLongerCover_An_AssignedCourse_Throws()
    {
        Coach coach = CreateCoach(["C#"]);
        Course course = CreateConfirmedCourse(requiredSkills: ["C#"]);
        course.AssignCoach(coach);
        Assert.Throws<CoachSkillUpdateWouldMakeAssignedCourseInvalid>(() => coach.UpdateSkills(["SQL"]));
    }

    [Fact]
    public void UpdatingSkills_While_StillCoveringAllAssignedCoursesSucceeds()
    {
        Coach coach = CreateCoach(["C#"]);
        Course course = CreateConfirmedCourse(requiredSkills: ["C#"]);
        course.AssignCoach(coach);
        coach.UpdateSkills(["C#", "SQL"]);
        Assert.Contains(Skill.From("SQL"), coach.Skills);
    }

    [Fact]
    public void Coach_With_NoAssignedCourses_CanUpdateSkillsFreely() =>
    Assert.Single(CreateCoach(["C#"]).UpdateSkills(["SQL"]).Skills);

}