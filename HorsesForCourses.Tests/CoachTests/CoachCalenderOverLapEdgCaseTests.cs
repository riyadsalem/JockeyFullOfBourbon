using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Courses.TimeSlots;

namespace HorsesForCourses.Tests.CoachTests;

public class CoachCalenderOverLapEdgCaseTests : TestHelpers
{
    [Fact]
    public void CoachIsAvailable_When_PeriodsOnlyTouchOnADay_ThatIsNotTheSharedWeekday()
    {
        Coach coach = CreateCoach(["C#"]);

        Course weekOne = CreateConfirmedCourse(
            start: new DateOnly(2026, 1, 5),
            end: new DateOnly(2026, 1, 11),
            day: CourseDay.Monday,
            startHour: 9,
            endHour: 10,
            requiredSkills: ["C#"]
        );
        weekOne.AssignCoach(coach);

        Course weekTwo = CreateConfirmedCourse(
            start: new DateOnly(2026, 1, 11),
            end: new DateOnly(2026, 1, 17),
            day: CourseDay.Monday,
            startHour: 9,
            endHour: 10,
            requiredSkills: ["C#"]
        );

        Assert.True(coach.IsAvailableFor(weekTwo));
    }
}