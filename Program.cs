namespace Mfumo
{
    using System;
    using System.Globalization;

    class Program
    {
        static void Main(string[] args)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.csv");
            IStorage storage = new FileStorage(file); 
            var manager = new StudentManager(storage);

            while (true)
            {
                Console.WriteLine("\nStudent Management");
                Console.WriteLine("1) Add student");
                Console.WriteLine("2) Edit student");
                Console.WriteLine("3) Display all");
                Console.WriteLine("4) Save");
                Console.WriteLine("5) Load");
                Console.WriteLine("0) Exit");
                Console.Write("Choice: ");
                var choice = Console.ReadLine()?.Trim();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        var newStudent = ReadStudentInput();
                        manager.AddStudent(newStudent);
                        break;
                    case "2":
                        Console.Write("Enter student Id to edit: ");
                        if (int.TryParse(Console.ReadLine(), out var id))
                        {
                            var existing = manager.FindById(id);
                            if (existing == null)
                            {
                                Console.WriteLine("Student not found.");
                                break;
                            }
                            Console.WriteLine("Leave input blank to keep current value.");
                            Console.Write($"First name ({existing.FirstName}): ");
                            var fn = Console.ReadLine();
                            Console.Write($"Last name ({existing.LastName}): ");
                            var ln = Console.ReadLine();
                            Console.Write($"DOB yyyy-MM-dd ({existing.DateOfBirth:yyyy-MM-dd}): ");
                            var dobIn = Console.ReadLine();
                            Console.Write($"Course ({existing.Course}): ");
                            var course = Console.ReadLine();

                            manager.EditStudent(id, s =>
                            {
                                if (!string.IsNullOrWhiteSpace(fn)) s.FirstName = fn;
                                if (!string.IsNullOrWhiteSpace(ln)) s.LastName = ln;
                                if (!string.IsNullOrWhiteSpace(dobIn) && DateTime.TryParseExact(dobIn, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                                    s.DateOfBirth = d;
                                if (!string.IsNullOrWhiteSpace(course)) s.Course = course;
                            });
                            Console.WriteLine("Student updated.");
                        }
                        else Console.WriteLine("Invalid id.");
                        break;
                    case "3":
                        manager.DisplayAll();
                        break;
                    case "4":
                        manager.Save();
                        Console.WriteLine("Saved to file.");
                        break;
                    case "5":
                        manager.Load();
                        Console.WriteLine("Loaded from file.");
                        break;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }

            
            // helper local function demonstrates functions and scope
            static Student ReadStudentInput()
            {
                Console.Write("First name: ");
                var fn = Console.ReadLine() ?? "";
                Console.Write("Last name: ");
                var ln = Console.ReadLine() ?? "";
                DateTime dob;
                while (true)
                {
                    Console.Write("DOB (yyyy-MM-dd): ");
                    var d = Console.ReadLine();
                    if (DateTime.TryParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob)) break;
                    Console.WriteLine("Invalid date format.");
                }
                Console.Write("Course: ");
                var crs = Console.ReadLine() ?? "";

                return new Student
                {
                    FirstName = fn,
                    LastName = ln,
                    DateOfBirth = dob,
                    Course = crs,
                };
            }
        }

    }
}