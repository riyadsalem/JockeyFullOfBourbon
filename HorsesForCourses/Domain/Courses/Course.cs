using HorsesForCourses.Abstractions;
using HorsesForCourses.Domain.Courses.InvalidationReasons;
using HorsesForCourses.Domain.Courses.TimeSlots;
using HorsesForCourses.Domain.Skills;
using HorsesForCourses.ValidationHelpers;
using HorsesForCourses.Domain.Coaches;

namespace HorsesForCourses.Domain.Courses;

public class Course : DomainEntity<Course>
{
    public CourseName Name { get; init; } = CourseName.Empty;

    public Period Period { get; init; } = Period.Empty;

    public IReadOnlyList<TimeSlot> TimeSlots => timeSlots.AsReadOnly();
    private readonly List<TimeSlot> timeSlots = [];

    public IReadOnlySet<Skill> RequiredSkills => requiredSkills.ToHashSet();
    private readonly HashSet<Skill> requiredSkills = [];

    public bool IsConfirmed { get; private set; }
    public Coach? AssignedCoach { get; private set; }

    private Course(string name, DateOnly start, DateOnly end)
    {
        Name = new CourseName(name);
        Period = Period.From(start, end);
    }

    public static Course Create(string name, DateOnly start, DateOnly end) => new(name, start, end);

    /* // Defet 4
    public override bool Equals(object? obj) => obj is Course other && Name == other.Name && Period == other.Period;
    public override int GetHashCode() => HashCode.Combine(Name, Period);
    */


    void NotAllowedIfAlreadyConfirmed() { if (IsConfirmed) throw new CourseAlreadyConfirmed(); }

    public virtual Course UpdateRequiredSkills(IEnumerable<string> newSkills)
    {
        // Store all skills in a list so they can be checked and used again without losing any skills
        List<String> skillNames = [.. newSkills];
        NotAllowedIfAlreadyConfirmed();
        NotAllowedWhenThereAreDuplicateSkills();
        return OverwriteRequiredSkills();
        void NotAllowedWhenThereAreDuplicateSkills()
        // Defect (3)
            => skillNames.NoDuplicatesAllowed(a => new CourseAlreadyHasSkill(string.Join(",", a)));
        Course OverwriteRequiredSkills()
        /* Validate all skills with Skill.From in validated skills before clearing the existing skill.... 
        If any value is invalid... nothing changes (((either everything succeeds or NOTHING changes....)))
        */
        {
            List<Skill> validatedSkills = [.. skillNames.Select(Skill.From)];
            requiredSkills.Clear();
            // Use the stored skills instead of reading newSkills again
            foreach (Skill s in validatedSkills) requiredSkills.Add(s);
            return this;
        }
    }

    public virtual Course UpdateTimeSlots<T>(
        IEnumerable<T> timeSlotInfo,
        Func<T, (CourseDay Day, int Start, int End)> getTimeSlot)
    {
        NotAllowedIfAlreadyConfirmed(); // Defect (2)
        var newTimeSlots = TimeSlot.EnumerableFrom(timeSlotInfo, getTimeSlot).ToList();
        NotAllowedWhenTimeSlotsOverlap();
        return OverwriteTimeSlots();
        void NotAllowedWhenTimeSlotsOverlap()
        {
            if (TimeSlot.HasOverlap(newTimeSlots))
                throw new OverlappingTimeSlots();
        }
        Course OverwriteTimeSlots()
        {
            timeSlots.Clear();
            timeSlots.AddRange(newTimeSlots);
            return this;
        }
    }

    public Course Confirm()
    {
        NotAllowedIfAlreadyConfirmed();
        NotAllowedWhenThereAreNoTimeSlots();
        return ConfirmIt();
        void NotAllowedWhenThereAreNoTimeSlots()
        {
            if (TimeSlots.Count == 0)
                throw new AtLeastOneTimeSlotRequired();
        }
        Course ConfirmIt() { IsConfirmed = true; return this; }
    }

    public virtual Course AssignCoach(Coach coach)
    {
        NotAllowedIfNotYetConfirmed();
        NotAllowedIfCourseAlreadyHasCoach();
        NotAllowedIfCoachIsUnsuitable(coach);
        NotAllowedIfCoachIsUnavailable(coach);
        return AssignTheCoachAlready(coach);
        void NotAllowedIfNotYetConfirmed()
        {
            if (!IsConfirmed)
                throw new CourseNotYetConfirmed();
        }
        void NotAllowedIfCourseAlreadyHasCoach()
        {
            if (AssignedCoach != null)
                throw new CourseAlreadyHasCoach();
        }
        void NotAllowedIfCoachIsUnsuitable(Coach coach)
        {
            if (!coach.IsSuitableFor(this))
                throw new CoachNotSuitableForCourse();
        }
        void NotAllowedIfCoachIsUnavailable(Coach coach)
        {
            if (!coach.IsAvailableFor(this))
                throw new CoachNotAvailableForCourse();
        }
        Course AssignTheCoachAlready(Coach coach)
        {
            AssignedCoach = coach;
            coach.AssignCourse(this);
            return this;
        }
    }
}