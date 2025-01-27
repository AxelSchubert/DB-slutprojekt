using System;
using System.Collections.Generic;

namespace AxelSchubertDB.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public string SocialSecurityNumber { get; set; } = null!;

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
