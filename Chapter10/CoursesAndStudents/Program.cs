using CoursesAndStudents.Models;
using Microsoft.EntityFrameworkCore;

namespace CoursesAndStudents {
    internal class Program {
        static async Task Main(string[] args) {
            using (Academy academy = new()) {
                bool deleted = await academy.Database.EnsureDeletedAsync();
                Console.WriteLine($"Database deleted: {deleted}.");
                bool created = await academy.Database.EnsureCreatedAsync();
                Console.WriteLine($"Database deleted {created}.");
                Console.WriteLine("SQL script used to create database:");
                Console.WriteLine(academy.Database.GenerateCreateScript());
                foreach (Student std in academy.Students.Include(s => s.Courses)) {
                    Console.WriteLine($"{std.FirstName} {std.LastName} attends the following {std.Courses.Count} courses:");
                    foreach (Course crs in std.Courses) {
                        Console.WriteLine($"{crs.Title}");
                    }
                }
            }
        }
    }
}
