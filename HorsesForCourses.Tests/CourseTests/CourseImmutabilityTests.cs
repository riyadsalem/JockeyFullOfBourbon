using HorsesForCourses.Domain.Courses.InvalidationReasons;
using HorsesForCourses.Domain.Courses.TimeSlots;

namespace HorsesForCourses.Tests.CourseTests;

public class CourseImmutabilityTests : TestHelpers
{
    [Fact]
    public void UpdatingSkills_On_ConfirmedCourse_Throws_CourseAlreadyConfirmed() =>
        Assert.Throws<CourseAlreadyConfirmed>(() => CreateConfirmedCourse().UpdateRequiredSkills(["C#"]));

    [Fact]
    public void UpdatingTimeSlots_On_ConfirmedCourse_Throws_CourseAlreadyConfirmed() =>
        Assert.Throws<CourseAlreadyConfirmed>(() => CreateConfirmedCourse().UpdateTimeSlots([(CourseDay.Thursday, 9, 10)], t => t));

    [Fact]
    // Even When the new data is invalid, a confirmed course must reject the change becausse it is already confirmed....
    public void UpdagingTimeSlots_On_ConfirmedCourse_With_InvalidDataStill_Throws_CourseAlreadyConfirmed() =>
        Assert.Throws<CourseAlreadyConfirmed>(() => CreateConfirmedCourse().UpdateTimeSlots([(CourseDay.Monday, 20, 21)], t => t));
    // CourseAlreadyConfirmed Exception....... (InvalidOfficeHour) > NEE
    // 20 is out of office hours
    // if (value < 9 || value > 17) throw new InvalidOfficeHour();



}