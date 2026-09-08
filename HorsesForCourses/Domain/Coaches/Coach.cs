using HorsesForCourses.Abstractions;
using HorsesForCourses.Domain.Coaches.InvalidationReasons;
using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Skills;
using HorsesForCourses.ValidationHelpers;
using HorsesForCourses.Domain.Courses.InvalidationReasons;

namespace HorsesForCourses.Domain.Coaches;

public class Coach : DomainEntity<Coach>
{
    public CoachName Name { get; init; } = CoachName.Empty;
    public CoachEmail Email { get; init; } = CoachEmail.Empty;

    public IReadOnlySet<Skill> Skills => skills.ToHashSet();
    private readonly HashSet<Skill> skills = [];

    public IReadOnlyList<Course> AssignedCourses => assignedCourses.AsReadOnly();
    private readonly List<Course> assignedCourses = [];

    private Coach(string name, string email)
    {
        Name = new CoachName(name);
        Email = new CoachEmail(email);
    }

    public static Coach Create(string name, string email) => new(name, email);
    public override bool Equals(object? obj) => obj is Coach other && Name == other.Name && Email == other.Email;
    public override int GetHashCode() => HashCode.Combine(Name, Email);

    public virtual Coach UpdateSkills(IEnumerable<string> newSkills)
    {
        List<String>? skillNames = [.. newSkills]; // One time
        NotAllowedWhenThereAreDuplicateSkills();
        NotAllowedIfAssignedCoursesWouldBecomeInvalid(); // Defect (4)
        OverwriteSkills();
        return this;
        void NotAllowedWhenThereAreDuplicateSkills()
        // Defect (3)
        // Here is loooop on newSkills (1 time)
            => skillNames.NoDuplicatesAllowed(a => new CoachAlreadyHasSkill(string.Join(",", a)));
        void NotAllowedIfAssignedCoursesWouldBecomeInvalid() // Defect (4)!!!!!!!!!!
        {
            var newSkills = skillNames.Select(Skill.From).ToHashSet();
            foreach (Course course in assignedCourses)
            {
                if (!course.RequiredSkills.All(newSkills.Contains))
                    throw new CoachSkillUpdateWouldMakeAssignedCourseInvalid();
            }
        }
        void OverwriteSkills()
        {
            List<Skill> validatedSkills = [.. skillNames.Select(Skill.From)];
            skills.Clear();
            validatedSkills.ForEach(a => skills.Add(a));
        }
    }

    // FIXXXX >>> was (Any) must be (All) A coach is suitable only if they cover EVERY REQUIRED SKILL..... not just one of them
    public bool IsSuitableFor(Course course)
        // => course.RequiredSkills.Any(Skills.Contains);
        => course.RequiredSkills.All(Skills.Contains); // Defect (1)

    public bool IsAvailableFor(Course course)
        => CheckIf.ImAvailable(this).For(course);
    // This is design patteren called Specification patteren, which separates the COMPLEX VALIDATION logic from the underlying class.

    public void AssignCourse(Course course)
    {
        if (!course.IsConfirmed)
            throw new CourseNotYetConfirmed();

        if (!ReferenceEquals(course.AssignedCoach, this))
            throw new CoachCourseAssignmentOutOfSync();

        if (assignedCourses.Contains(course))
            return;

        assignedCourses.Add(course);
    }
}
