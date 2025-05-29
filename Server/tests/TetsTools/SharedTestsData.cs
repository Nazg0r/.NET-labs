using System.Text;
using Modules.Students.Domain.Entities;
using Modules.Works.Domain.Entities;
using Modules.Works.IntegrationEvents;

namespace TestsTools
{
    public static class SharedTestsData
    {
        public static readonly List<Student> TestStudents =
        [
            new Student
            {
                Id = Guid.NewGuid(),
                Username = "johnDoe33",
                Name = "John",
                Surname = "Doe",
                Group = "IM-00",
            },

            new Student
            {
                Id = Guid.NewGuid(),
                Username = "walterWhite",
                Name = "Walter",
                Surname = "White",
                Group = "IM-01",
            }
        ];

        public static readonly List<GetStudentWorksResponse> TestStudentsWorks =
        [
            new GetStudentWorksResponse()
            {
                Works =
                [
                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("hello world"),
                        FileName = "lab1.cs",
                        LoadDate = DateTime.UtcNow,
                    },
                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("Change the world"),
                        FileName = "homework.js",
                        LoadDate = DateTime.UtcNow,
                    },

                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("hello my friend"),
                        FileName = "email.txt",
                        LoadDate = DateTime.UtcNow,
                    },
                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("a lot of code"),
                        FileName = "program.cs",
                        LoadDate = DateTime.UtcNow,
                    }
                ]
            },
            new GetStudentWorksResponse()
            {
                Works =
                [
                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("hello world"),
                        FileName = "myStudentWorkDto.cs",
                        LoadDate = DateTime.UtcNow,
                    },

                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("I do it tomorrow"),
                        FileName = "course.java",
                        LoadDate = DateTime.UtcNow,
                    },

                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes("hello world 2"),
                        FileName = "lab2.cs",
                        LoadDate = DateTime.UtcNow,
                    },

                    new StudentWorkDto
                    {
                        Id = Guid.NewGuid(),
                        Content = Encoding.UTF8.GetBytes(""),
                        FileName = "empty.txt",
                        LoadDate = DateTime.UtcNow,
                    }
                ]
            }
        ];

        public static readonly List<Work> WorksWithSameExtension =
        [
            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("home work"),
                FileName = "original",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("my home work"),
                FileName = "copy",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("task"),
                FileName = "irrelevant",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("home work"),
                FileName = "copy",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("home task"),
                FileName = "extra",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            }
        ];

        public static readonly List<Work> WorksWithDifferentExtension =
        [
            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("home work"),
                FileName = "original",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("my home work"),
                FileName = "copy",
                Extension = ".txt",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("task"),
                FileName = "irrelevant",
                Extension = ".cs",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            },

            new Work
            {
                Id = Guid.NewGuid(),
                Content = Encoding.UTF8.GetBytes("home task"),
                FileName = "extra",
                Extension = ".c",
                LoadDate = DateTime.Now.ToUniversalTime(),
                StudentId = Guid.NewGuid().ToString()
            }
        ];
    }
}