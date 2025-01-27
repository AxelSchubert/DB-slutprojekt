using AxelSchubertDB.Data;
using AxelSchubertDB.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        //Meny som kallar metoderna för de olika funktionerna
        bool isRunning = true;
        while (isRunning)
        {
            Console.Clear();
            Console.WriteLine("Välkommen till Skoladministratören! Välj ett alternativ från menyn:\n1.Visa anställda\n2.Visa elever\n" +
                "3.Visa elever i vald kurs\n4.Visa senaste betyg\n5.Visa kurser\n6.Visa en elevs kurser och betyg\n7.Lägg till elev\n8.Lägg till anställd");
            int.TryParse(Console.ReadLine(), out int choice);
            switch (choice)
            {
                case 1:
                    DisplayEmployees();
                    Console.ReadKey();
                    break;
                case 2:
                    Console.WriteLine("Vill du se eleverna i 1. alfabetisk ordning eller 2. omvänd alfabetisk ordning?");
                    var sortingChoice = Console.ReadLine();
                    if (sortingChoice != "1" && sortingChoice != "2")
                        Console.WriteLine("Ogiltigt val.");
                    else
                    {
                        DisplayStudents(sortingChoice == "1");
                    }
                    Console.ReadKey();
                    break;
                case 3:
                    DisplayStudentsByCourse();
                    Console.ReadKey();
                    break;
                case 4:
                    DisplayLatestGrades();
                    Console.ReadKey();
                    break;
                case 5:
                    DisplayCourses();
                    Console.ReadKey();
                    break;
                case 6:
                    DisplayStudents(true);
                    Console.WriteLine("Skriv in ElevID på eleven vars kurser du vill se: ");
                    int.TryParse(Console.ReadLine(), out int studentId);
                    DisplayCoursesByStudent(studentId);
                    Console.ReadKey();
                    break;
                case 7:
                    var newStudent = new Student();
                    Console.WriteLine("Ange elevens namn:");
                    newStudent.FullName = Console.ReadLine();
                    Console.WriteLine("Ange elevens kontaktinfo:");
                    newStudent.ContactInfo = Console.ReadLine();
                    Console.WriteLine("Ange elevens personnummer:");
                    newStudent.SocialSecurityNumber = Console.ReadLine();
                    AddStudent(newStudent);
                    Console.WriteLine("Eleven är nu tillagd i databasen");
                    Console.ReadKey();
                    break;
                case 8:
                    var newEmployee = new Employee();
                    Console.WriteLine("Ange anställdes namn:");
                    newEmployee.FullName = Console.ReadLine();
                    Console.WriteLine("Ange anställdes yrke:");
                    newEmployee.Occupation = Console.ReadLine();
                    Console.WriteLine("Ange anställdes kontaktinfo:");
                    newEmployee.ContactInfo = Console.ReadLine();
                    AddEmployee(newEmployee);
                    Console.WriteLine("Den nyanställda är nu tillagd i databasen");
                    Console.ReadKey();
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }
    }
    //Redovisar alla anställda
    static void DisplayEmployees()
    {
        using (var context = new SchoolSystemContext())
        {
            var employees = context.Employees.ToList().GroupBy(e => e.Occupation).OrderBy(g => g.Key); //Jag förstår inte riktigt varför jag måste lägga ToList() innan det andra men annars funkar det inte
            foreach (var group in employees)
            {
                int? divisionSalary = 0;
                Console.WriteLine($"Här är alla anställda med befattningen {group.Key}:");
                foreach (var employee in group)
                {
                    divisionSalary += employee.Salary;
                    Console.WriteLine(employee.FullName);
                }
                Console.WriteLine($"Totalt betals {divisionSalary} kr ut till denna avdelning varje månad och medellönen är {divisionSalary / group.Count()}");
            }
        }
    }
    //Visar alla studenter och tar in en bool för att bestämma hur listan ska ordnas
    static void DisplayStudents(bool ascOrDesc)
    {
        using (var context = new SchoolSystemContext())
        {
            var students = context.Students.OrderBy(s => s.FullName).ToList();
            if (!ascOrDesc)
            {
                students = context.Students.OrderByDescending(s => s.FullName).ToList();
            }
            Console.WriteLine("Här är alla studenter:");
            foreach (var student in students)
            {
                Console.WriteLine($"{student.FullName} - ElevID: {student.StudentId}");
            }
        }
    }
    //Visar alla studenter i en viss kurs
    static void DisplayStudentsByCourse()
    {
        using (var context = new SchoolSystemContext())
        {
            var courses = context.Courses.ToList();
            Console.WriteLine("Vilken kurs vill du välja? (Skriv in ID på kursen du vill välja.)");
            foreach (var course in courses)
            {
                Console.WriteLine($"ID: {course.CourseId} Kursnamn: {course.CourseName}");
            }
            int.TryParse(Console.ReadLine(), out int courseId);
            //Använder join med länkningstabellen StudentCourses för att ta fram alla elever som går en viss kurs
            var studentNames = context.StudentCourses
                                .Where(sc => sc.CourseIdFk == courseId)
                                .Join(context.Students,
                                      sc => sc.StudentIdFk,
                                      s => s.StudentId,
                                      (sc, s) => new { s.FullName })
                                .ToList();
            Console.WriteLine("Här är alla elever i vald kurs:");
            foreach (var student in studentNames)
            {
                Console.WriteLine(student.FullName);
            }
        }
    }
    
    static void DisplayCoursesByStudent(int studentId)
    {
        using (var context = new SchoolSystemContext())
        {
            var courseIds = context.StudentCourses.Where(sc => sc.StudentIdFk == studentId).Select(sc => sc.CourseIdFk).ToList();
            Console.WriteLine("Eleven går följande kurser:");
            foreach (var Id in courseIds)
            {
                var course = context.Courses.Find(Id);
                var grade = context.StudentCourses.Where(sc => sc.StudentIdFk == studentId && sc.CourseIdFk == Id).Select(sc => new
                {
                    sc.GradeDate,
                    sc.Grade
                })
    .First(); //Tar fram betyg och datum genom att hitta raden i StudentCourse som pekar på både aktuell student och kurs.
                string gradingTeacherName = context.Employees.Find(course.TeacherIdFk).FullName; //Tar fram lärare som ansvarar för den aktuella kursen
                Console.WriteLine($"Kurs: {course.CourseName} | Betyg: {grade.Grade} | Betygsatt av: {gradingTeacherName} | Datum: {grade.GradeDate}");
            }

        }
    }
    static void DisplayLatestGrades()
    {
        using (var context = new SchoolSystemContext())
        {
            DateOnly dateMonthAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)); //Variabel med datumet för en månad sen
            var studentCourses = context.StudentCourses.Where(sc => sc.GradeDate > dateMonthAgo)
                                .Join(context.Students,
                                      sc => sc.StudentIdFk,
                                      s => s.StudentId,
                                      (sc, s) => new { s.FullName, sc.Grade, sc.GradeDate })
                                .ToList();
            Console.WriteLine("Här är alla betyg satta den senaste månaden:");
            foreach (var studentCourse in studentCourses)
            {
                Console.WriteLine($"{studentCourse.FullName} - {studentCourse.Grade} - {studentCourse.GradeDate}");
            }
        }
    }
    //Visar alla kurser och statistik om betygen
    static void DisplayCourses()
    {
        using (var context = new SchoolSystemContext())
        {
            var courseStatistics = context.StudentCourses.GroupBy(sc => sc.CourseIdFk)
                                   .Select(group => new
                                   {
                                       CourseID = group.Key,
                                       CourseName = context.Courses.Where(c => c.CourseId == group.Key).Select(c => c.CourseName).FirstOrDefault(), //Eftersom group.Key är fk till courses använder jag den för att få fram kursnamnet
                                       AvgGrade = AverageGrade(group.Select(sc => sc.Grade).ToList()),
                                       MaxGrade = group.Min(sc => sc.Grade),
                                       MinGrade = group.Max(sc => sc.Grade)
                                   });
            foreach (var course in courseStatistics)
            {
                Console.WriteLine($"KursID: {course.CourseID} Kursnamn: {course.CourseName} Högsta betyg: {course.MaxGrade} Lägsta betyg: {course.MinGrade} Snittbetyg: {course.AvgGrade}");
            }
        }
    }
    //Metod för att räkna ut genomsnittet på en lista med betyg
    static string AverageGrade(List<string> grades)
    {
        var gradeToNumber = new Dictionary<string, int> //Dictionary för att omvandla a-f till 5-0 för att kunna räkna utt genomsnitt
            {
                {"A", 5},
                {"B", 4},
                {"C", 3},
                {"D", 2},
                {"E", 1},
                {"F", 0}
            };
        List<int> convertedGrades = grades.Select(g => gradeToNumber[g]).ToList();
        double average = convertedGrades.Average();
        var gradeAverage = gradeToNumber.FirstOrDefault(x => x.Value == (int)Math.Round(average)).Key;
        return gradeAverage;
    }
    //Metod för att lägga till en ny student
    static void AddStudent(Student student)
    {
        using (var context = new SchoolSystemContext())
        {
            context.Students.Add(student);
            context.SaveChanges();
        }

    }
    //Metod för att lägga till en ny anställd
    static void AddEmployee(Employee employee)
    {
        using (var context = new SchoolSystemContext())
        {
            context.Employees.Add(employee);
            context.SaveChanges();
        }
    }
}