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
