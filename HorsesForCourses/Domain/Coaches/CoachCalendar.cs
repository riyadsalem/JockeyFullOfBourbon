using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Courses.TimeSlots;

namespace HorsesForCourses.Domain.Coaches;

public static class CheckIf
{
    public static CoachCalendar ImAvailable(Coach coach) => new(coach);
}

public class CoachCalendar(Coach coach)
{
    private readonly Coach coach = coach;

    public bool For(Course course)
    {
        foreach (var assigned in coach.AssignedCourses)
        {
            if (CoursesOverlap(course, assigned))
            {
                return false;
            }
        }
        return true;
    }

    private static bool CoursesOverlap(Course courseOne, Course courseTwo)
    {
        var start = Max(courseOne.Period.Start, courseTwo.Period.Start);
        var end = Min(courseOne.Period.End, courseTwo.Period.End);
        if (end < start) return false;

        var courseOneByDay = GetTimeSlotsByDay(courseOne);
        var courseTwoByDay = GetTimeSlotsByDay(courseTwo);

        foreach (CourseDay day in Enum.GetValues(typeof(CourseDay)))
        {
            // if this day is not within the overlap period, ignore it and do not check its TimeSlots...
            if (!HasDay(start, end, day)) continue;
            if (!courseOneByDay.TryGetValue(day, out var slotsOne))
                continue;

            if (!courseTwoByDay.TryGetValue(day, out var slotsTwo))
                continue;

            foreach (var slotOne in slotsOne)
                foreach (var slotTwo in slotsTwo)
                    if (slotOne.OverlapsWith(slotTwo))
                        return true;
        }

        return false;
    }

    private static readonly DayOfWeek[] DaysInOrder =
    [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];

    private static bool HasDay(DateOnly start, DateOnly end, CourseDay day)
    {
        DayOfWeek target = DaysInOrder[(int)day];
        DateOnly current = start;
        while (current <= end)
        {
            if (current.DayOfWeek == target) return true;
            current = current.AddDays(1);
        }
        return false;
    }
    private static Dictionary<CourseDay, List<TimeSlot>> GetTimeSlotsByDay(Course courseOne)
    {
        return courseOne.TimeSlots
            .GroupBy(t => t.Day)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static DateOnly Max(DateOnly x, DateOnly y) => x > y ? x : y;
    private static DateOnly Min(DateOnly x, DateOnly y) => x < y ? x : y;
}