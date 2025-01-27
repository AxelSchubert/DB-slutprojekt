using System;
using System.Collections.Generic;

namespace AxelSchubertDB.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public int? TeacherIdFk { get; set; }

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual Employee? TeacherIdFkNavigation { get; set; }
}
