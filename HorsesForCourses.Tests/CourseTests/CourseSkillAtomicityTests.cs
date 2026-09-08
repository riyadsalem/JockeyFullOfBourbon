using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Skills;

namespace HorsesForCourses.Tests.CourseTests;

public class CourseSkillAtomicityTests : TestHelpers
{
    [Fact]
    public void UpdateRequiredSkills_With_AnInvalidSkill_DoesNotChangeExistingSkills()
    {
        Course course = CreateunconfirmedCourse(["C#"]);
        Assert.ThrowsAny<Exception>(() => course.UpdateRequiredSkills(["SQL", ""]));
        Assert.Equal([Skill.From("C#")], course.RequiredSkills);
    }
}