### Defect 1 – Coach Suitability (DONE)
Changed `Any` to `All` in **`Coach.cs`** to ensure a coach is only considered suitable when they have **all required skills**.
I will also add tests to verify and prove that the new behavior works correctly.

### Defect 2 – Course Immutability (DONE)
Changed the order in **`Course.cs`** inside `UpdateTimeSlots`.
`NotAllowedIfAlreadyConfirmed()` is now checked **before** validating the new timeslot data. This ensures that a confirmed course cannot be modified, even when the new timeslot data is invalid.
I will also add tests to verify and prove that the correct `CourseAlreadyConfirmed` exception is thrown.

### Defect 3 – Coach Skill Update Invariant (DONE)
Changed the implementation in **`Coach.cs`** so that `UpdateSkills` now checks whether the coach is currently assigned to any course before applying the change. If the new skills would no longer cover the required skills of an already assigned course, the update is rejected with a new exception, `CoachSkillUpdateWouldMakeAssignedCourseInvalid`. This ensures that a coach can never end up assigned to a course they are no longer suitable for, even through a valid sequence of otherwise correct operations (assign coach, then update their skills).
I will also add tests to verify and prove that updating skills in a way that would invalidate an assigned course throws correctly, while updates that keep the coach suitable still succeed.

### Defect 4 – Coach/Course Identity Equality (DONE)
Added `Equals`/`GetHashCode` overrides to both **`Coach.cs`** and **`Course.cs`**.
Previously, neither class defined its own equality, so both inherited the base **`DomainEntity<T>`** equality, which compares entities by `Id`. Since this project has no persistence, every entity's `Id` stays empty forever. This meant that two entities were never considered equal, even when they had identical data.
This contradicted the requirements, which state that a **Coach is identified by `CoachName` and `CoachEmail`**, while a **Course is identified by `CourseName` and `Period`**.
Equality now compares these identifying properties directly.
I will also add tests to verify and prove that:
* Two coaches with the same name and email are considered equal.
* Two courses with the same name and period are considered equal.
* Coaches or courses with different identifying values are correctly considered not equal.

### Defect 5 – Skill Update Atomicity (DONE)
Changed **`Coach.cs`** (`UpdateSkills`) and **`Course.cs`** (`UpdateRequiredSkills`) to validate all new skills into a separate list before clearing the existing skills.
Previously, skills were cleared first and validated one by one, so an invalid value could throw an exception halfway through and leave the entity with partially updated skills.
The update is now atomic: **either all skills are valid and the update succeeds, or nothing changes**.
I will also add tests to verify that invalid skill values throw an exception and leave the existing skills unchanged for both `Coach` and `Course`.

### Defect 6 – CoachCalendar.CoursesOverlap Edge Case (DONE)
Changed **`CoachCalendar.cs`** inside `CoursesOverlap`.
Previously, the code checked separately whether the course periods overlap and whether they have the same weekday with overlapping hours. It did not verify that the shared weekday actually occurs within the overlapping date range.
This caused a false overlap when two periods only touched on a day that was not the shared weekday. For example, two courses could both have a Monday session while their periods only touch on a Sunday. The algorithm would report a conflict even though the Monday sessions are one week apart.
Added `HasDay` to ensure the shared weekday actually occurs within the overlapping date range.
I will also add a test to verify that this edge case returns no conflict, while real same-week conflicts are still detected correctly.

