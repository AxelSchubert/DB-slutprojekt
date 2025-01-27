using System;
using System.Collections.Generic;

namespace AxelSchubertDB.Models;

public partial class StudentCourse
{
    public int StudentCourseId { get; set; }

    public string Grade { get; set; } = null!;

    public DateOnly GradeDate { get; set; }

    public int StudentIdFk { get; set; }

    public int CourseIdFk { get; set; }

    public virtual Course CourseIdFkNavigation { get; set; } = null!;

    public virtual Student StudentIdFkNavigation { get; set; } = null!;
}
