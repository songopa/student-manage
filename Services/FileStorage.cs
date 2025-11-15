namespace Mfumo
{
    using System;

    interface IStorage
    {
        void Save(IEnumerable<Student> students);
        List<Student> Load();
    }

    class FileStorage : IStorage
    {
        private readonly string _filePath;
        public FileStorage(string filePath) => _filePath = filePath;

        public void Save(IEnumerable<Student> students)
        {
            using var w = new StreamWriter(_filePath, false);
            // header
            w.WriteLine("Id,FirstName,LastName,DOB,Course");
            foreach (var s in students)
                w.WriteLine(s.ToCsv());
        }

        public List<Student> Load()
        {
            var list = new List<Student>();
            if (!File.Exists(_filePath)) return list;
            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines.Skip(1)) // skip header
            {
                if (Student.TryFromCsv(line, out var s) && s is Student st)
                    list.Add(st);
            }
            return list;
        }
    }

}