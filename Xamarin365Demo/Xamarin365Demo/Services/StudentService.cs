using System.Collections.Generic;

using Xamarin365Demo.Models;

namespace Xamarin365Demo.Services
{
    class StudentService
    {
        public static List<Student> GetStudents()
        {
            return new List<Student>()
            {
                new Student()
                {
                    Id = 1,
                    Name = "Luis Beltrán",
                    Code = "01031014",
                    Faculty = "Applied Informatics"
                },
                new Student()
                {
                    Id = 2,
                    Name = "Ana Martínez",
                    Code = "10071622",
                    Faculty = "Technology"
                },
                new Student()
                {
                    Id = 3,
                    Name = "Juan Pérez",
                    Code = "16062056",
                    Faculty = "Management"
                },
                new Student()
                {
                    Id = 4,
                    Name = "Sara Aguirre",
                    Code = "09020471",
                    Faculty = "Applied Informatics"
                },
                new Student()
                {
                    Id = 5,
                    Name = "Roberto Díaz",
                    Code = "20010102",
                    Faculty = "Management"
                }
            };
        }
    }
}
