namespace Mfumo
{
    using System;
    using System.Globalization;
    using System.IO;
    class Student : Person
    {
        public int Id { get; set; }
        public string Course { get; set; } = "";
        public DateTime DateOfBirth { get; set; }

        // computed property (expression)
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public override string ToString()
        {
            return $"[{Id}] {FirstName} {LastName}, Age: {Age}, DOB: {DateOfBirth:yyyy-MM-dd}, Course: {Course}";
        }

        public string ToCsv()
        {
            // simple CSV escaping: replace commas
            string esc(string s) => (s ?? "").Replace(",", ";");
            return $"{Id},{esc(FirstName)},{esc(LastName)},{DateOfBirth:yyyy-MM-dd},{esc(Course)}";
        }

        public static bool TryFromCsv(string line, out Student? student)
        {
            student = null;
            if (string.IsNullOrWhiteSpace(line)) return false;
            var parts = line.Split(',');
            if (parts.Length < 6) return false;
            if (!int.TryParse(parts[0], out var id)) return false;
            if (!DateTime.TryParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob)) return false;

            student = new Student
            {
                Id = id,
                FirstName = parts[1].Replace(";", ","),
                LastName = parts[2].Replace(";", ","),
                DateOfBirth = dob,
                Course = parts[4].Replace(";", ",")
            };
            return true;
        }
    }

    class StudentManager
    {
        private readonly List<Student> _students = new();
        private readonly IStorage _storage;
        private int _nextId = 1;

        public StudentManager(IStorage storage)
        {
            _storage = storage;
            Load();
            if (_students.Any()) _nextId = _students.Max(s => s.Id) + 1;
        }

        public void AddStudent(Student s)
        {
            s.Id = _nextId++;
            _students.Add(s);
            Console.WriteLine("Student added.");
        }

        public bool EditStudent(int id, Action<Student> editAction)
        {
            var s = _students.FirstOrDefault(x => x.Id == id);
            if (s == null) return false;
            editAction(s);
            return true;
        }

        public void DisplayAll()
        {
            if (!_students.Any())
            {
                Console.WriteLine("No students found.");
                return;
            }
            foreach (var s in _students)
                Console.WriteLine(s);
        }

        public void Save() => _storage.Save(_students);

        public void Load()
        {
            var loaded = _storage.Load();
            _students.Clear();
            _students.AddRange(loaded);
        }

        public Student? FindById(int id) => _students.FirstOrDefault(s => s.Id == id);
    }
}