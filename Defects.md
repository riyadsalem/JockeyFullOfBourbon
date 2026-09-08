### Defect 1 – Coach Suitability (DONE)

Changed `Any` to `All` in **`Coach.cs`** to ensure a coach is only considered suitable when they have **all required skills**.
I will also add tests to verify and prove that the new behavior works correctly.

### Defect 2 – Course Immutability (DONE)

Changed the order in **`Course.cs`** inside `UpdateTimeSlots`.
`NotAllowedIfAlreadyConfirmed()` is now checked **before** validating the new timeslot data. This ensures that a confirmed course cannot be modified, even when the new timeslot data is invalid.
I will also add tests to verify and prove that the correct `CourseAlreadyConfirmed` exception is thrown.

### Defect 3 – Skills Silently Disappear....

Changed the implementation in **`Coach.cs`** and **`Course.cs`** to materialize the given skills once at the beginning of the method.
This ensures that **all given skills are stored correctly**, even when the input is a single-pass `IEnumerable<string>`.
I will also add tests to verify and prove that all given skills are kept correctly..... 

### Defect 4 – Coach Skill Update Invariant (DONE)

Changed the implementation in **`Coach.cs`** so that `UpdateSkills` now checks whether the coach is currently assigned to any course before applying the change. If the new skills would no longer cover the required skills of an already assigned course, the update is rejected with a new exception, `CoachSkillUpdateWouldMakeAssignedCourseInvalid`. This ensures that a coach can never end up assigned to a course they are no longer suitable for, even through a valid sequence of otherwise correct operations (assign coach, then update their skills).
I will also add tests to verify and prove that updating skills in a way that would invalidate an assigned course throws correctly, while updates that keep the coach suitable still succeed.

### Defect 5 – Coach/Course Identity Equality (DONE)

Added `Equals`/`GetHashCode` overrides to both **`Coach.cs`** and **`Course.cs`**.
Previously, neither class defined its own equality, so both inherited the base **`DomainEntity<T>`** equality, which compares entities by `Id`. Since this project has no persistence, every entity's `Id` stays empty forever. This meant that two entities were never considered equal, even when they had identical data.
This contradicted the requirements, which state that a **Coach is identified by `CoachName` and `CoachEmail`**, while a **Course is identified by `CourseName` and `Period`**.
Equality now compares these identifying properties directly.
I will also add tests to verify and prove that:
* Two coaches with the same name and email are considered equal.
* Two courses with the same name and period are considered equal.
* Coaches or courses with different identifying values are correctly considered not equal.
