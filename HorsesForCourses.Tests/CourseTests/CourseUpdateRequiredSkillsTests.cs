using HorsesForCourses.Domain.Courses.InvalidationReasons;
using HorsesForCourses.Domain.Skills;

namespace HorsesForCourses.Tests.CourseTests;

public class CourseUpdateRequiredSkillsTests : TestHelpers
{
    [Fact]
    public void UpdateRequiredSkills_Should_Be_Atomic()
    {
        var course = CreateunconfirmedCourse(["C#"]);
        Assert.Throws<CourseAlreadyHasSkill>(() => course.UpdateRequiredSkills(["Duplicate", "Duplicate"]));
        Assert.Equal([Skill.From("C#")], course.RequiredSkills);
    }
}