# Domain Requirements & Behavioral Rules

The domain models a lightweight training management system centered around **Coaches** and **Courses**. It captures essential domain rules without introducing persistence, UI, or infrastructure concerns.

## Core Concepts

`Coach` and `Course` are entities. Their equality is based on their **`Id`**, not on the descriptive properties listed below.

### Coach
* Has a **`CoachName`** and a **`CoachEmail`**.
* Skills are a **set**: duplicates are rejected.
* Suitability: a coach is suitable for a course only if they have **all required skills** of that course.
* Availability: a coach is available if the new course **does not overlap** with any of their already assigned courses (period + timeslot overlap rules below).

### Course
* Has a **`CourseName`** and a **`Period`** (`Start`, `End`).
* **TimeSlots**: one or more, each on a weekday in office hours (see constraints).
* **No overlapping timeslots** within the same course.
* **Confirmation** requires at least one timeslot. After confirmation, the course becomes **immutable** (no edits).
* **Assigning a coach** is only allowed when:
  * The course has **no existing coach**.
  * The course is **confirmed**.
* A course can only have a **valid coach** assigned to it (suitable and available):
  * The coach is **suitable** (skills cover course requirements).
  * The coach is **available** (no overlap with their assigned courses).

### Skill

* Value object wrapping a **non-empty string**.

### Period

* `Start` and `End` must be present and ordered (`Start <= End`).

### OfficeHour

* Integer hour within **09:00–17:00** (inclusive). Used by `TimeSlot`.

### CourseDay

* Weekdays only: **Monday–Friday**.

## Validation

* Value objects enforce local validation at construction (e.g., non-empty, length constraints).
* Guard clauses at method boundaries enforce domain rules (e.g., no edits after confirmation, no overlapping timeslots).
* Failures are explicit via descriptive exceptions (e.g., `CourseAlreadyConfirmed`, `OverlappingTimeSlots`, `CoachNotAvailableForCourse`).

## Overlap & Availability Rules (Summary)

* Two courses **overlap** for a coach when:
  1. Their **date periods** intersect, **and**
  2. They share at least one **weekday** where there exists a **timeslot overlap** (`Start < Other.End` and `End > Other.Start`).
* A coach is **unavailable** for a new course if such an overlap exists with any of their currently assigned courses.

