using HorsesForCourses.Abstractions;
using HorsesForCourses.Domain.Coaches;
using HorsesForCourses.Domain.Courses;
using HorsesForCourses.Domain.Courses.TimeSlots;
using HorsesForCourses.Domain.Skills;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Coach table
        modelBuilder.Entity<Coach>(coach =>
        {
            coach.Property(c => c.Id)
            .HasConversion(id => id.Value, // TO DB (SAVE)
            value => Id<Coach>.From(value)) // FROM DB (READ)
            .ValueGeneratedOnAdd();

            coach.OwnsOne(c => c.Name, name =>
            name.Property(n => n.Value)
            .HasColumnName("Name")
            .HasMaxLength(DefaultString.MaxLength));

            coach.OwnsOne(c => c.Email, email =>
            email.Property(n => n.Value)
            .HasColumnName("Email")
            .HasMaxLength(DefaultString.MaxLength));

            /* Skills is a list of value objects (not a single value)
            so it needs its own small table instead of a simple column conversion
            */
            coach.OwnsMany(typeof(Skill), "skills", skill =>
            // The Coach owns many Skills, and "skills" is the private field that contains them....
            // voor lezen
            // "skills" >> private field name
            {
                skill.ToTable("CoachSkills");
                skill.WithOwner().HasForeignKey("CoachId");
                /*
                Create an int Id property for the Skill in the database...
                This Id does not exist in the skill domain object....
                */
                skill.Property<int>("Id");
                skill.HasKey("Id");
                skill.Property<string>("Value").HasColumnName("Value");
                // Do not allow the same Coach to have same Skill twicccce.
                skill.HasIndex("CoachId", "Value").IsUnique();
            });
            // Tell EF Core to use the private field "skills" directly instead of the public property "Skills"
            // voor opslag en bewerking
            //  private readonly HashSet<Skill> skills = [];
            coach.Navigation("skills").UsePropertyAccessMode(PropertyAccessMode.Field);
            coach.Ignore(c => c.Skills); // public computed view,,,, hot a separate navigation

            // Database >> Ef Core >> assignedCourses (Private field) >> AssignedCourses (you read the daata from it)
            coach.Navigation(nameof(Coach.AssignedCourses)).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // Course table
        modelBuilder.Entity<Course>(course =>
        {
            course.Property(c => c.Id)
            .HasConversion(id => id.Value, value => Id<Course>.From(value))
            .ValueGeneratedOnAdd();

            course.OwnsOne(c => c.Name, name =>
            name.Property(n => n.Value).HasColumnName("Name")
            .HasMaxLength(DefaultString.MaxLength));

            // Period has two values (Start, End) so it maps to two colums..
            course.OwnsOne(c => c.Period, period =>
            {
                period.Property(p => p.Start).HasColumnName("PeriodStart");
                period.Property(p => p.End).HasColumnName("PeriodEnd");
            });

            course.OwnsMany(typeof(Skill), "requiredSkills", skill =>
            // OwnsMany ((((((Maak een table aan in de database...))))))
            {
                skill.ToTable("CourseRequiredSkills");
                skill.WithOwner().HasForeignKey("CourseId");
                skill.Property<int>("Id");
                skill.HasKey("Id");
                skill.Property<string>("Value").HasColumnName("Value");
                skill.HasIndex("CourseId", "Value").IsUnique();
            });
            course.Navigation("requiredSkills").UsePropertyAccessMode(PropertyAccessMode.Field);
            course.Ignore(c => c.RequiredSkills);

            // TimeSlots >> also a list... each slot has Day + Start + End
            course.OwnsMany(typeof(TimeSlot), "timeSlots", slot =>
            {
                slot.ToTable("CourseTimeSlots");
                slot.WithOwner().HasForeignKey("CourseId");
                slot.Property<int>("Id");
                slot.HasKey("Id");

                slot.Property(nameof(TimeSlot.Day))
                .HasColumnName("Day")
                .HasConversion<string>();

                slot.Property<OfficeHour>(nameof(TimeSlot.Start))
                .HasColumnName("StartHour")
                .HasConversion(h => h.Value, v => OfficeHour.From(v));

                slot.Property<OfficeHour>(nameof(TimeSlot.End))
                .HasColumnName("EndHour")
                .HasConversion(h => h.Value, v => OfficeHour.From(v));
            });
            course.Navigation("timeSlots").UsePropertyAccessMode(PropertyAccessMode.Field);
            course.Ignore(c => c.TimeSlots);

            course.HasOne(c => c.AssignedCoach)
            .WithMany(c => c.AssignedCourses)
            .HasForeignKey("AssignedCoachId")
            .IsRequired(false);
        });
    }
}

/*
Coaches tabel 
Id
Name
Email
CoachSkills (id, CoachId, Value)
*********************
Courses tabel
Id
Name
periodStart
PeriodEnd
IsConfirmed...............................
AssignedCoachId
CourseRequiredSkills (id, CourseId, Value)
CourseTimeSlots (id, CourseId, Day, StartHour, EndHour)
*********************
Coaches(1) TO (*) CoachSkills {{Via CoachId}}
Coaches(1) TO (*) Courses {{Via AssignedCoachId}}
Courses(1) TO (*) CourseRequiredSkills {{Via CourseId}}
Courses(1) TO (*) CourseTimeSlots {{Via CourseId}}
*/